using System.Data.Common;

namespace DataAccess.Readers
{
    public abstract class EntityReaderBase<T>
    {
        public DbDataReader Reader { get; private set; }

        public virtual void SetReader(DbDataReader value)
        {
            Reader = value;
        }

        protected EntityReaderBase(DbDataReader reader)
        {
            Reader = reader;
        }

        protected EntityReaderBase()
        {
        }

        public abstract T ReadEntity();
    }
}
