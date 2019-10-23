using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Mel.Live.Extensions;
using Mel.Live.Extensions.UnityExtensions;
using Mel.Live.Models.Entities;
using Mel.Live.Models.Entities.BackOffice;
using Mel.Live.Models.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDbGenericRepository;
using MongoDbGenericRepository.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Mel.Live.Data
{
    [AutoBuild]
    public class MongoDataContext : MongoDbContext
    {
        #region Properties

        #region Collections
        public Company Company => Store.GetOne<Company>(c => true);

        public IMongoCollection<User> Users => GetCollection<User>();
        public IMongoCollection<ResourceBase> Resources => GetCollection<ResourceBase>();
        /*
        public IMongoCollection<Wallet> Wallets => GetCollection<Wallet>();
        public IMongoCollection<Transaction> Transactions => GetCollection<Transaction>();
        public IMongoCollection<Bet> Bets => GetCollection<Bet>();
        public IMongoCollection<Bank> Banks => GetCollection<Bank>();
        public IMongoCollection<PaymentChannel> PaymentChannels => GetCollection<PaymentChannel>();
        */
        #endregion

        #region Services
        [DeepDependency]
        public IMongoRepository Store { get; }

        [DeepDependency]
        ILogger Logger { get; }

        [DeepDependency(TargetType = typeof(IOptions<MongoDBOptions>), TargetProperty = nameof(IOptions<MongoDBOptions>.Value))]
        MongoDBOptions Options { get; }
        #endregion

        #region Statics
        public static MongoDataContext Current { get; private set; }
        #endregion

        #endregion

        #region Constructors
        public MongoDataContext(MongoClient client, string databaseName) : base(client, databaseName) { Current = this; }
        public MongoDataContext(string connectionString, string databaseName) : base(connectionString, databaseName) { Current = this; }
        #endregion

        #region Methods
        public IEnumerable<TDoc> Get<TDoc>(ICollection<Guid> source, Expression<Func<TDoc, bool>> filter = null) where TDoc : Document
        {
            if (filter == null) filter = o => source.Contains(o.Id);
            else filter = filter.CombineWithAndAlso(o => source.Contains(o.Id));

            return Store.GetAll(filter);
        }

        public IEnumerable<TDoc> Get<TDoc, TKey>(ICollection<TKey> source, Expression<Func<TDoc, bool>> filter = null) where TKey : IEquatable<TKey> where TDoc : IDocument<TKey>
        {
            if (filter == null) filter = o => source.Contains(o.Id);
            else filter = filter.CombineWithAndAlso(o => source.Contains(o.Id));

            return Store.GetAll<TDoc, TKey>(filter);
        }

        public async Task<IEnumerable<TDoc>> GetAsync<TDoc, TKey>(ICollection<TKey> source, Expression<Func<TDoc, bool>> filter = null) where TKey : IEquatable<TKey> where TDoc : IDocument<TKey>
        {
            if (filter == null) filter = o => source.Contains(o.Id);
            else filter = filter.CombineWithAndAlso(o => source.Contains(o.Id));
            
            return await Store.GetAllAsync<TDoc, TKey>(filter);
        }

        public async Task<IEnumerable<TDoc>> GetAsync<TDoc>(ICollection<Guid> source, Expression<Func<TDoc, bool>> filter = null) where TDoc : Document
        {
            if (filter == null) filter = o => source.Contains(o.Id);
            else filter = filter.CombineWithAndAlso(o => source.Contains(o.Id));

            return await Store.GetAllAsync(filter);
        }
        #endregion
    }
}
