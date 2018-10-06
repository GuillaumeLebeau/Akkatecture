﻿// The MIT License (MIT)
//
// Copyright (c) 2018 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq.Expressions;
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Akka;
using Akkatecture.Configuration.DependancyInjection;
using Akkatecture.Core;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AkkatectureServiceCollectionExtensions
    {

        public static IAkkatectureBuilder AddAkkatecture(
            this IServiceCollection services,
            ActorSystem actorSystem)
        {
            services.AddSingleton<ActorSystem>(actorSystem);
            return new AkkatectureBuilder(services,actorSystem);
        }
        
        public static IAkkatectureBuilder AddAggregateManager<TAggregateManager, TAggregate, TIdentity>(
            this IAkkatectureBuilder builder)
            where TAggregateManager : ReceiveActor, IAggregateManager<TAggregate, TIdentity>
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            var aggregateManager = builder.ActorSystem.ActorOf(Props.Create<TAggregateManager>());
            var actorRef = new ActorRefOfT<TAggregateManager>(aggregateManager);

            builder.Services.AddSingleton<IActorRef<TAggregateManager>>(actorRef);
            return builder;
        }

        public static IAkkatectureBuilder AddAggregateManager<TAggregateManager>(
            this IAkkatectureBuilder builder,
            IActorRef aggregateManagerRef)
            where TAggregateManager : ReceiveActor, IAggregateManager
        {
            var actorRef = new ActorRefOfT<TAggregateManager>(aggregateManagerRef);
            //var actorRef = aggregateManagerRef as IActorRef<TAggregateManager>;
            builder.Services.AddSingleton<IActorRef<TAggregateManager>>(actorRef);
            return builder;
        }
        
        public static IAkkatectureBuilder AddAggregateManager<TAggregateManager, TAggregate, TIdentity>(
            this IAkkatectureBuilder builder, Expression<Func<TAggregateManager>> aggregateManagerFactory)
            where TAggregateManager : ReceiveActor, IAggregateManager<TAggregate, TIdentity>
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            var aggregateManager = builder.ActorSystem.ActorOf(Props.Create(aggregateManagerFactory));
            var actorRef = new ActorRefOfT<TAggregateManager>(aggregateManager);

            builder.Services.AddSingleton<IActorRef<TAggregateManager>>(actorRef);
            return builder;
        }

        public static IAkkatectureBuilder AddSagaManager<TAggregateSagaManager, TAggregateSaga, TIdentity, TSagaLocator>(
            this IAkkatectureBuilder builder, Expression<Func<TAggregateSaga>> sagaFactory)
            where TAggregateSagaManager : ActorBase, IAggregateSagaManager<TAggregateSaga, TIdentity, TSagaLocator>
            where TAggregateSaga : IAggregateSaga<TIdentity>
            where TIdentity : SagaId<TIdentity>
            where TSagaLocator : class, ISagaLocator<TIdentity>
        {
            var sagaManager = builder.ActorSystem.ActorOf(Props.Create<TAggregateSagaManager>(sagaFactory));
            var actorRef = new ActorRefOfT<TAggregateSagaManager>(sagaManager);

            builder.Services.AddSingleton<IActorRef<TAggregateSagaManager>>(actorRef);
            return builder;
        }
        
        public static IAkkatectureBuilder AddSagaManager<TAggregateSagaManager, TAggregateSaga, TIdentity, TSagaLocator>(
            this IAkkatectureBuilder builder, IActorRef sagaManagerRef)
            where TAggregateSagaManager : ActorBase, IAggregateSagaManager<TAggregateSaga, TIdentity, TSagaLocator>
            where TAggregateSaga : IAggregateSaga<TIdentity>
            where TIdentity : SagaId<TIdentity>
            where TSagaLocator : class, ISagaLocator<TIdentity>
        {
            var actorRef = new ActorRefOfT<TAggregateSagaManager>(sagaManagerRef);

            builder.Services.AddSingleton<IActorRef<TAggregateSagaManager>>(actorRef);
            return builder;
        }
        
        
        public static IAkkatectureBuilder AddActorReference<TActor>(
            this IAkkatectureBuilder builder, IActorRef actorReference)
            where TActor : ActorBase
        {
            var actorRef = new ActorRefOfT<TActor>(actorReference);

            builder.Services.AddSingleton<IActorRef<TActor>>(actorRef);
            return builder;
        }
        
    }
}