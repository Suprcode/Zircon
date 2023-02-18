using System;

namespace Server.Envir.Commands.Exceptions {
    class UserCommandFatalException : Exception {
        public UserCommandFatalException(string message) : base(message) {
        }
    }
}
