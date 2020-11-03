﻿namespace UniModules.UniContextData.Runtime.Entities {
    using System;
    using System.Collections.Generic;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Connections;
    using UniCore.Runtime.Common;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniGame.Core.Runtime.DataFlow;
    using UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniRx;

    [Serializable]
    public class EntityContext :
        IContext {
        private TypeData           data;
        private LifeTimeDefinition lifeTimeDefinition;
        private TypeDataBrodcaster broadcaster;
        private int                id;

        public EntityContext() {
            //context data container
            data = new TypeData();
            //context lifetime
            lifeTimeDefinition = new LifeTimeDefinition();
            broadcaster        = new TypeDataBrodcaster();

            id = Unique.GetId();
        }

        #region connection api

        public int ConnectionsCount => broadcaster.ConnectionsCount;

        public void Disconnect(IMessagePublisher connection) {
            broadcaster.Disconnect(connection);
        }

        public IDisposable Bind(IMessagePublisher connection) {
            var disposable = broadcaster.Bind(connection);
            return disposable;
        }

        #endregion

        #region public properties

        public int Id => id;

        public ILifeTime LifeTime => lifeTimeDefinition.LifeTime;

        public bool HasValue => data.HasValue;

        #endregion

        #region public methods

        public bool Contains<TData>() {
            return data.Contains<TData>();
        }

        public virtual TData Get<TData>() {
            return data.Get<TData>();
        }

        public bool Remove<TData>() {
            return data.Remove<TData>();
        }

        public void Release() {
            data.Release();
            broadcaster.Release();
            lifeTimeDefinition.Release();
        }

        public virtual void Dispose() {
            Release();
            this.Despawn();
        }

        #region rx

        public void Publish<T>(T message) {
            data.Publish(message);
            broadcaster.Publish(message);
        }

        public IObservable<T> Receive<T>() {
            return data.Receive<T>();
        }

        #endregion

        #endregion

        #region Unity Editor Api

#if UNITY_EDITOR

        public IReadOnlyDictionary<Type, IValueContainerStatus> EditorValues => data.EditorValues;

#endif

        #endregion
    }
}