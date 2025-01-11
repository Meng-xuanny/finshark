using Microsoft.EntityFrameworkCore;
using api.Models;


namespace api.Data{
public class ApplicationDBContext: DbContext{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> dbContextOptions):base(dbContextOptions)
    {
        
    }

    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Comment> Comments { get; set; }

}
}