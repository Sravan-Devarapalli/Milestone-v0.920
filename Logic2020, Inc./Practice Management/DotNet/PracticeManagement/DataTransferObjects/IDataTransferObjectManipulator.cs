namespace DataTransferObjects
{
    /// <summary>
    /// Provides basic operations for given entity
    /// </summary>
    /// <typeparam name="TE"></typeparam>
    public interface IDataTransferObjectManipulator<TE> where TE : class, IIdNameObject, new()
    {
        /// <summary>
        /// Get entity by Id
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <returns>Entity instance</returns>
        TE GetById(int id);

        /// <summary>
        /// Insert entity to the database
        /// </summary>
        /// <param name="entity">Entity instance</param>
        /// <returns>Id of the inserted entity</returns>
        int Add(TE entity);

        /// <summary>
        /// Remove entity from the database
        /// </summary>
        /// <param name="entity">Entity instance</param>
        void Remove(TE entity);


        /// <summary>
        /// Update entity in the database
        /// </summary>
        void Update(TE entity);
    }
}
