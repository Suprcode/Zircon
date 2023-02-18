using System;

namespace Server.Envir.Commands.Exceptions {
    public class UserCommandException : Exception {
        private bool UserOnly;

        public UserCommandException(string message, bool userOnly = false) : base(message) {
            UserOnly = userOnly;
        }

        public bool userOnly {
            get { return UserOnly; }
        }
    }
}
