using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Other;
using DataAccess.Readers;
using DataTransferObjects;

namespace DataAccess.Generic
{
    public abstract class DalBase<TE, TR> : IDataTransferObjectManipulator<TE> where TE : class, IIdNameObject, new() 
        where TR : EntityReaderBase<TE>, new()
    {
        #region Procedure Names

        protected abstract string GetByIdProcedure { get; }
        protected abstract string AddProcedure { get; }
        protected abstract string UpdateProcedure { get; }
        protected abstract string RemoveProcedure { get; }

        #endregion

        #region Initializers

        protected abstract SqlParameter InitAddCommand(TE entity, SqlCommand command);
        protected abstract void InitRemoveCommand(TE entity, SqlCommand command);
        protected abstract void InitUpdateCommand(TE entity, SqlCommand command);
        protected abstract void InitGetById(TE entity, SqlCommand command);

        #endregion

        #region Method body templates

        protected static void ExecNonQueryNoReturnParameters(TE entity, string procedureName, Action<TE, SqlCommand> commandInitialization)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(procedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                commandInitialization.Invoke(entity, command);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        protected static int ExecNonQueryWithReturnParameter(TE entity, string procedureName, Func<TE, SqlCommand, SqlParameter> commandInitialization)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(procedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                var returnParam = commandInitialization.Invoke(entity, command);

                connection.Open();

                command.ExecuteNonQuery();

                return Convert.ToInt32(returnParam.Value);
            }
        }

        protected TE[] ExecuteReader(TE entity, string procedureName, Action<TE, SqlCommand> commandInitialization)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(procedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                commandInitialization(entity, command);

                connection.Open();

                using (var reader = command.ExecuteReader())
                    return (new List<TE>(Read(reader))).ToArray();
            }
        }

        #endregion

        #region Basic operations

        public virtual TE GetById(int id)
        {
            var items = ExecuteReader(new TE {Id = id}, GetByIdProcedure, InitGetById);
            return items.Length == 0 ? null : items[0];
        }

        public virtual int Add(TE entity)
        {
            return ExecNonQueryWithReturnParameter(entity, AddProcedure, InitAddCommand);
        }

        public virtual void Remove(TE entity)
        {
            ExecNonQueryNoReturnParameters(entity, RemoveProcedure, InitRemoveCommand);
        }

        public virtual void Update(TE entity)
        {
            ExecNonQueryNoReturnParameters(entity, UpdateProcedure, InitUpdateCommand);
        }

        #endregion

        #region Reading

        protected IEnumerable<TE> Read(DbDataReader reader)
        {
            var entityReader = new TR();
            entityReader.SetReader(reader);

            if (reader.HasRows)
                while (reader.Read())
                    yield return entityReader.ReadEntity();
        }

        protected abstract EntityReaderBase<TE> InitEntityReader(DbDataReader reader);

        #endregion
    }
}

