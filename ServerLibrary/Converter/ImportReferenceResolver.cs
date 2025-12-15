using MirDB;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Server
{
    public static class ImportReferenceResolver
    {
        private sealed class ReferenceContext
        {
            public DBObject Owner { get; init; }
            public PropertyInfo Property { get; init; }
        }

        private sealed class DeferredReference
        {
            public DBObject Owner { get; init; }
            public PropertyInfo Property { get; init; }
            public Type ReferenceType { get; init; }
            public List<string> IdentityValues { get; init; }
        }

        [ThreadStatic]
        private static ReferenceContext _currentContext;

        private static readonly List<DeferredReference> PendingReferences = new();

        public static bool EnableDeferredResolution { get; set; } = true;

        public static void SetContext(DBObject owner, PropertyInfo property)
        {
            _currentContext = new ReferenceContext
            {
                Owner = owner,
                Property = property
            };
        }

        public static void ClearContext()
        {
            _currentContext = null;
        }

        public static bool TryAddMissingReference(Type referenceType, List<string> identityValues)
        {
            if (!EnableDeferredResolution || _currentContext == null)
            {
                return false;
            }

            lock (PendingReferences)
            {
                PendingReferences.Add(new DeferredReference
                {
                    Owner = _currentContext.Owner,
                    Property = _currentContext.Property,
                    ReferenceType = referenceType,
                    IdentityValues = identityValues.ToList()
                });
            }

            SEnvir.Log($"Deferred missing reference for '{referenceType.Name}' using values '{string.Join('/', identityValues)}' on '{_currentContext.Owner?.GetType().Name}.{_currentContext.Property?.Name}'.");

            return true;
        }

        public static (int resolved, int remaining) ResolvePendingReferences(Session session)
        {
            List<DeferredReference> pendingSnapshot;

            lock (PendingReferences)
            {
                pendingSnapshot = PendingReferences.ToList();
            }

            if (pendingSnapshot.Count == 0)
            {
                return (0, 0);
            }

            List<DeferredReference> stillPending = new();
            int resolved = 0;

            foreach (var reference in pendingSnapshot)
            {
                DBObject resolvedReference = TryResolveReference(session, reference);

                if (resolvedReference != null)
                {
                    reference.Property.SetValue(reference.Owner, resolvedReference);
                    resolved++;

                    SEnvir.Log($"Resolved deferred reference for '{reference.Owner.GetType().Name}.{reference.Property.Name}'.");
                }
                else
                {
                    stillPending.Add(reference);

                    SEnvir.Log($"Pending reference still missing for '{reference.ReferenceType.Name}' using values '{string.Join('/', reference.IdentityValues)}' on '{reference.Owner.GetType().Name}.{reference.Property.Name}'.");
                }
            }

            lock (PendingReferences)
            {
                PendingReferences.Clear();
                PendingReferences.AddRange(stillPending);
            }

            return (resolved, stillPending.Count);
        }

        private static DBObject TryResolveReference(Session session, DeferredReference reference)
        {
            var converterType = typeof(DBObjectReferenceConverter<>).MakeGenericType(reference.ReferenceType);

            object converter = Activator.CreateInstance(converterType, session);

            MethodInfo getObjectMethod = converterType.GetMethod("GetObjectFromIdentity", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            return getObjectMethod?.Invoke(converter, new object[] { reference.IdentityValues, false }) as DBObject;
        }
    }
}