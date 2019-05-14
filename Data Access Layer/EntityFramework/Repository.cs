using NeYemekYapsam.Common;
using NeYemekYapsam.Core.DataAccess;
using NeYemekYapsam.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NeYemekYapsam.DataAccessLayer.EntityFramework
{
    public class Repository<T>: RepositoryBase, IDataAccess<T> where T:class
    {
       
        private DbSet<T> _objectSet;

        public Repository()
        {
           
            _objectSet =context.Set<T>();
        }

        public List<T> List()
        {
           return _objectSet.ToList();
        }

        public List<T> List(Expression<Func<T,bool>>where)
        {
            return _objectSet.Where(where).ToList();
        }

        public int Insert(T obj)
        {
            _objectSet.Add(obj);
            if (obj is NYYEntityBase)
            {
                NYYEntityBase o = obj as NYYEntityBase;
                DateTime now = DateTime.Now;
                o.CreatedOn = now;
                o.ModifiedOn = now;
                o.ModifiedUsername = App.Common.GetCurrentUsername();
            }

            return Save();
        }

        public IQueryable<T> ListQueryable()
        {
            return _objectSet.AsQueryable();
        }

        public int Update(T obj)
        {

            if (obj is NYYEntityBase)
            {
                NYYEntityBase o = obj as NYYEntityBase;
             
            
                o.ModifiedOn = DateTime.Now;
                o.ModifiedUsername = App.Common.GetCurrentUsername();
            }

            return Save();
        }

        public int Delete(T obj)
        {
           


            _objectSet.Remove(obj);
            return Save();
            
        }

        public int Save()
        {
            return context.SaveChanges();
        }

        public T Find(Expression<Func<T, bool>> where)
        {
            return _objectSet.FirstOrDefault(where);
        }
    }
}
