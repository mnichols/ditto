﻿using System;
using Castle.Core.Logging;
using Ditto.Internal;

namespace Ditto.WindsorIntegration
{
    public class WindsorLoggerFactory : ILogFactory
    {
        private readonly ILoggerFactory loggerFactory;

        public WindsorLoggerFactory(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }
        public ILog Create(Type type)
        {
            return new WindsorLogger(loggerFactory.Create(type));
        }

        public ILog Create(object instance)
        {
            return new WindsorLogger(loggerFactory.Create(instance.GetType()));
        }
    }
}