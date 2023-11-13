using BooKStore.Models.Domain;
using BooKStore.Models.DTO;
using BooKStore.Repository.Abstract;

namespace BooKStore.Repository.Implementation
{
    public class AuthorService : IAuthorService
    {
        private readonly DataBaseContext context;
        public AuthorService(DataBaseContext context)
        {
            this.context = context;
        }
        public bool Add(Author model)
        {
            try
            {
                context.Author.Add(model);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var data = this.FindById(id);
                if (data == null)
                    return false;
                context.Author.Remove(data);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Author FindById(int id)
        {
            return context.Author.Find(id);
        }

        public IEnumerable<Author> GetAll()
        {
            return context.Author.ToList();
        }

        public bool Update(Author model)
        {
            try
            {
                context.Author.Update(model);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
