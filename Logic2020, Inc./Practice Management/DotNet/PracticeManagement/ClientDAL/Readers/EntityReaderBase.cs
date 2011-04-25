using System.Data.Common;

namespace DataAccess.Readers
{
    public abstract class EntityReaderBase<T>
    {
        private DbDataReader _reader;

        public DbDataReader Reader
        {
            get { return _reader; }
        }

        public virtual void SetReader(DbDataReader value)
        {
            _reader = value;
        }

        protected EntityReaderBase(DbDataReader reader)
        {
            _reader = reader;
        }

        protected EntityReaderBase()
        {
        }

        public abstract T ReadEntity();
    }
}
