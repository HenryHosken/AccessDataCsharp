using BaltaDataAccess.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BaltaDataAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            const string connectionString = "Server=localhost, 1433;Database=balta;User ID=sa;Password=H5nry@Hosken!S5norio;Trusted_Connection=False; TrustServerCertificate=True;";

            using (var connection = new SqlConnection(connectionString))
            {

            }
        }

        static void ListCategories(SqlConnection connection)
        {
            var categories = connection.Query<Category>("SELECT [Id], [Title] FROM [Category]");
            foreach (var item in categories)
            {
                Console.WriteLine($"{item.Id} - {item.Title}");
            }

        }

        static void CreateCategory(SqlConnection connection)
        {
            var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Description = "Categoria de serviços do AWS";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;

            // SQL Injection

            var insertSql = @"INSERT INTO 
                    [Category] 
                VALUES(
                    @Id,    
                    @Title,
                    @Url,
                    @Summary, 
                    @Order,  
                    @Description,
                    @Featured)";

            var rows = connection.Execute(insertSql, new
            {
                category.Id,
                category.Title,
                category.Url,
                category.Summary,
                category.Order,
                category.Description,
                category.Featured
            });
            Console.WriteLine($"{rows} linhas inseridas");
        }
    }
}
