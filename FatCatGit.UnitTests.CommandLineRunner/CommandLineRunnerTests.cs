﻿using System.Collections.Generic;
using FatCatGit.CommandLineRunner;
using NUnit.Framework;

namespace FatCatGit.UnitTests.CommandLineRunner
{
    [TestFixture]
    [Category("Command Line Runner")]
    public class CommandLineRunnerTests
    {
        [Test, MaxTime(125)]
        public void CommandLineRunnerWillRunOperationsAsync()
        {
            var command = new ConsoleCommand
                              {
                                  CommandFullLocation = "CommandLineUnitTester",
                                  Arguments = "wait"
                              };

            var runner = new ConsoleRunner(command);

            runner.BeginExecute();
        }

        [Test]
        public void CommandLineRunnerWillTriggerEventWhenErrorOutputRecieved()
        {
            var command = new ConsoleCommand
                              {
                                  CommandFullLocation = "CommandLineUnitTester"
                              };

            var runner = new ConsoleRunner(command);

            var dataCollection = new List<string>();

            runner.ErrorOutputReceived += e => dataCollection.Add(e.Data);

            runner.Execute();

            Assert.That(dataCollection.Contains("This is on the Error Stream"));
        }

        [Test]
        public void CommandLineRunnerWillTriggerEventWhenOutputRecieved()
        {
            var command = new ConsoleCommand
                              {
                                  CommandFullLocation = "CommandLineUnitTester"
                              };

            var runner = new ConsoleRunner(command);

            var dataCollection = new List<string>();

            runner.StandardOutputReceived += e => dataCollection.Add(e.Data);

            runner.Execute();

            Assert.That(dataCollection.Contains("This is echo base."));
        }

        [Test]
        public void CommandRunnerWillHaveErrorOutput()
        {
            var command = new ConsoleCommand
                              {
                                  CommandFullLocation = "CommandLineUnitTester",
                                  Arguments = "We read you red 5"
                              };

            var runner = new ConsoleRunner(command);

            runner.Execute();

            Assert.That(runner.ErrorOutput.Contains("This is on the Error Stream"));
        }

        [Test]
        public void CommandWillReturnOutputAsString()
        {
            var command = new ConsoleCommand
                              {
                                  CommandFullLocation = "CommandLineUnitTester"
                              };

            var runner = new ConsoleRunner(command);

            runner.Execute();

            Assert.That(runner.Output.Contains("This is echo base."), Is.True);
        }

        [Test]
        public void CommandWillRunInGivenDirectory()
        {
            var command = new ConsoleCommand
                              {
                                  CommandFullLocation = "CommandLineUnitTester",
                                  Arguments = "We read you red 5",
                                  WorkingDirectory = @"C:\Program Files"
                              };

            var runner = new ConsoleRunner(command);

            runner.Execute();

            Assert.That(runner.Output.Contains(@"C:\Program Files"), Is.True);
        }

        [Test]
        public void CommandWithArguments()
        {
            var command = new ConsoleCommand
                              {
                                  CommandFullLocation = "CommandLineUnitTester",
                                  Arguments = "We read you red 5",
                              };

            var runner = new ConsoleRunner(command);

            runner.Execute();

            Assert.That(runner.Output.Contains("Wereadyoured5"), Is.True);
        }
    }
}