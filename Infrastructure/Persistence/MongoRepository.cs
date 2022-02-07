using System.Linq.Expressions;
using Application.Interfaces;
using Domain.Interfarces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Persistence
{
    public class MongoRepository<TEntity> : IRepository<TEntity> where TEntity : IEntity
    {
        private readonly IMongoCollection<TEntity> _collection;

    public MongoRepository(IMongoDbSettings settings)
    {
        var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
        _collection = database.GetCollection<TEntity>(GetCollectionName(typeof(TEntity)));
    }

    private protected string GetCollectionName(Type documentType)
    {
        return ((BsonCollectionAttribute) documentType.GetCustomAttributes(
                typeof(BsonCollectionAttribute),
                true)
            .FirstOrDefault())?.CollectionName;
    }

    public virtual IQueryable<TEntity> AsQueryable()
    {
        return _collection.AsQueryable();
    }

    public virtual IEnumerable<TEntity> FilterBy(
        Expression<Func<TEntity, bool>> filterExpression)
    {
        return _collection.Find(filterExpression).ToEnumerable();
    }

    public virtual IEnumerable<TProjected> FilterBy<TProjected>(
        Expression<Func<TEntity, bool>> filterExpression,
        Expression<Func<TEntity, TProjected>> projectionExpression)
    {
        return _collection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
    }

    public virtual TEntity FindOne(Expression<Func<TEntity, bool>> filterExpression)
    {
        return _collection.Find(filterExpression).FirstOrDefault();
    }

    public virtual Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression)
    {
        return Task.Run(() => _collection.Find(filterExpression).FirstOrDefaultAsync());
    }

    public virtual TEntity FindById(string id)
    {
        var Guid = new Guid(id);
        var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, Guid);
        return _collection.Find(filter).SingleOrDefault();
    }

    public virtual Task<TEntity> FindByIdAsync(string id)
    {
        return Task.Run(() =>
        {
            var Guid = new Guid(id);
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, Guid);
            return _collection.Find(filter).SingleOrDefaultAsync();
        });
    }


    public virtual void InsertOne(TEntity document)
    {
        _collection.InsertOne(document);
    }

    public virtual Task InsertOneAsync(TEntity document)
    {
        return Task.Run(() => _collection.InsertOneAsync(document));
    }

    public void InsertMany(ICollection<TEntity> documents)
    {
        _collection.InsertMany(documents);
    }


    public virtual async Task InsertManyAsync(ICollection<TEntity> documents)
    {
        await _collection.InsertManyAsync(documents);
    }

    public void ReplaceOne(TEntity document)
    {
        var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, document.Id);
        _collection.FindOneAndReplace(filter, document);
    }

    public virtual async Task ReplaceOneAsync(TEntity document)
    {
        var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, document.Id);
        await _collection.FindOneAndReplaceAsync(filter, document);
    }

    public void DeleteOne(Expression<Func<TEntity, bool>> filterExpression)
    {
        _collection.FindOneAndDelete(filterExpression);
    }

    public Task DeleteOneAsync(Expression<Func<TEntity, bool>> filterExpression)
    {
        return Task.Run(() => _collection.FindOneAndDeleteAsync(filterExpression));
    }

    public void DeleteById(string id)
    {
        var Guid = new Guid(id);
        var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, Guid);
        _collection.FindOneAndDelete(filter);
    }

    public Task DeleteByIdAsync(string id)
    {
        return Task.Run(() =>
        {
            var Guid = new Guid(id);
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, Guid);
            _collection.FindOneAndDeleteAsync(filter);
        });
    }

    public void DeleteMany(Expression<Func<TEntity, bool>> filterExpression)
    {
        _collection.DeleteMany(filterExpression);
    }

    public Task DeleteManyAsync(Expression<Func<TEntity, bool>> filterExpression)
    {
        return Task.Run(() => _collection.DeleteManyAsync(filterExpression));
    }
    }
}