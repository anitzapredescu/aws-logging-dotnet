using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AWS.Logger.AspNetCore.Tests
{
    public class TestFormatter
    {
        [Theory]
        [InlineData("my log message", LogLevel.Trace)]
        [InlineData("my log message", LogLevel.Debug)]
        [InlineData("my log message", LogLevel.Critical)]
        public void CustomFormatter_Must_Be_Applied(string message, LogLevel logLevel)
        {
            Func<LogLevel, object, Exception, string> customFormatter
                = (level, state, ex) => level + " hello world" + state.ToString();

            Func<string, LogLevel, bool> filter = (categoryName, level) => true;

            var coreLogger = new FakeCoreLogger();

            var logger = new AWSLogger("TestCategory", coreLogger, filter, customFormatter);

            logger.Log(logLevel, 0, message, null, (state, ex) => state.ToString());

            string expectedMessage = customFormatter(logLevel, message, null);

            Assert.Equal(expectedMessage, coreLogger.ReceivedMessages.First().Replace("\r\n", string.Empty));
        }

        [Theory]
        [InlineData("my log message", LogLevel.Trace)]
        [InlineData("my log message", LogLevel.Debug)]
        [InlineData("my log message", LogLevel.Critical)]
        public void FormatterWithStateAndEx_Must_Be_Applied(string message, LogLevel logLevel)
        {
            Func<LogLevel, object, Exception, string> customFormatter
                = (level, state, ex) => level + " hello world" + state.ToString();

            Func<string, LogLevel, bool> filter = (categoryName, level) => true;

            var coreLogger = new FakeCoreLogger();

            var logger = new AWSLogger("TestCategory", coreLogger, filter, null);

            logger.Log(logLevel, 0, message, null, (state, ex) => state.ToString());

            string expectedMessage = message.ToString();

            Assert.Equal(message, coreLogger.ReceivedMessages.First().Replace("\r\n", string.Empty));
        }

        [Theory]
        [InlineData(1, "my log message", LogLevel.Trace)]
        [InlineData(2, "my log message", LogLevel.Debug)]
        [InlineData(3, "my log message", LogLevel.Critical)]
        public void FormatterWithStateAndEventIdAndEx_Must_Be_Applied(int eventId, string message, LogLevel logLevel)
        {
            Func<LogLevel, object, Exception, string> customFormatter
                = (level, state, ex) => level + " hello world" + state.ToString();

            Func<string, LogLevel, bool> filter = (categoryName, level) => true;

            var coreLogger = new FakeCoreLogger();

            var logger = new AWSLogger("TestCategory", coreLogger, filter, null);

            Func<string, EventId, Exception, string> formatter = (state, evId, ex) => $"{state.ToString()} - {evId.Id}";
            logger.Log<string>(logLevel, eventId, message, null, formatter);

            string expectedMessage = $"{message.ToString()} - {eventId}";

            Assert.Equal(expectedMessage, coreLogger.ReceivedMessages.First().Replace("\r\n", string.Empty));
        }
    }
}
