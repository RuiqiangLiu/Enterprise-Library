﻿#region license
// ==============================================================================
// Microsoft patterns & practices Enterprise Library
// Transient Fault Handling Application Block
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
#endregion

using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Data;

namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Tests.ReliableConnectionScenarios.given_successfull_execute_scalar_command
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using Common.TestSupport.ContextBase;

    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.TestSupport;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

    using VisualStudio.TestTools.UnitTesting;

    public abstract class Context : ArrangeActAssert
    {
        protected ReliableSqlConnection reliableConnection;
        protected TestRetryStrategy connectionStrategy;
        protected TestRetryStrategy commandStrategy;
        protected SqlCommand command;

        protected override void Arrange()
        {
            this.command = new SqlCommand(TestSqlSupport.ValidSqlScalarQuery);

            this.connectionStrategy = new TestRetryStrategy();

            this.commandStrategy = new TestRetryStrategy();

            this.reliableConnection = new ReliableSqlConnection(
                TestSqlSupport.SqlDatabaseConnectionString,
                new RetryPolicy<AlwaysTransientErrorDetectionStrategy>(this.connectionStrategy),
                new RetryPolicy<AlwaysTransientErrorDetectionStrategy>(this.commandStrategy));
        }
    }

    [TestClass]
    public class when_executing_command_with_no_connection : Context
    {
        private int result;

        protected override void Act()
        {
            this.result = this.reliableConnection.ExecuteCommand<int>(this.command);
        }

        [TestMethod]
        public void then_connection_is_closed()
        {
            Assert.IsTrue(this.command.Connection.State == ConnectionState.Closed);
        }

        [TestMethod]
        public void then_can_read_results()
        {
            Assert.IsTrue(this.result > 0);
        }

        [TestMethod]
        public void then_retried()
        {
            Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
            Assert.AreEqual(0, this.commandStrategy.ShouldRetryCount);
        }
    }

    [TestClass]
    public class when_executing_command_with_object_with_no_connection : Context
    {
        private int result;

        protected override void Act()
        {
            this.result = (int)this.reliableConnection.ExecuteCommand<object>(this.command);
        }

        [TestMethod]
        public void then_connection_is_closed()
        {
            Assert.IsTrue(this.command.Connection.State == ConnectionState.Closed);
        }

        [TestMethod]
        public void then_can_read_results()
        {
            Assert.IsTrue(this.result > 0);
        }

        [TestMethod]
        public void then_retried()
        {
            Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
            Assert.AreEqual(0, this.commandStrategy.ShouldRetryCount);
        }
    }

    [TestClass]
    public class when_executing_command_with_closed_connection : Context
    {
        private int result;

        protected override void Act()
        {
            this.command.Connection = new SqlConnection(TestSqlSupport.SqlDatabaseConnectionString);

            this.result = this.reliableConnection.ExecuteCommand<int>(this.command);
        }

        [TestMethod]
        public void then_connection_is_closed()
        {
            Assert.IsTrue(this.command.Connection.State == ConnectionState.Closed);
        }

        [TestMethod]
        public void then_can_read_results()
        {
            Assert.IsTrue(this.result > 0);
        }

        [TestMethod]
        public void then_retried()
        {
            Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
            Assert.AreEqual(0, this.commandStrategy.ShouldRetryCount);
        }
    }

    [TestClass]
    public class when_executing_command_with_opened_connection : Context
    {
        private int result;

        protected override void Act()
        {
            this.command.Connection = new SqlConnection(TestSqlSupport.SqlDatabaseConnectionString);
            this.command.Connection.Open();

            this.result = this.reliableConnection.ExecuteCommand<int>(this.command);
        }

        [TestMethod]
        public void then_connection_is_opened()
        {
            Assert.IsTrue(this.command.Connection.State == ConnectionState.Open);
        }

        [TestMethod]
        public void then_can_read_results()
        {
            Assert.IsTrue(this.result > 0);
        }

        [TestMethod]
        public void then_retried()
        {
            Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
            Assert.AreEqual(0, this.commandStrategy.ShouldRetryCount);
        }
    }
}