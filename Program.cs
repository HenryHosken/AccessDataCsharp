using BaltaDataAccess.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BaltaDataAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            const string connectionString = "Server=localhost, 1401;Database=balta;User ID=sa;Password=H5nry@Hosken@S5norio;Trusted_Connection=False; TrustServerCertificate=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                // UpdateCategory(connection);
                // CreateManyCategory(connection);
                // ListCategories(connection);
                // DeleteCategory(connection);
                // GetCategory(connection);
                // ExecuteReadProcedure(connection);
                // ExecuteScalar(connection);
                // ReadView(connection);
                //OneToOne(connection);
                OneToMany(connection);
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

        static void GetCategory(SqlConnection connection)
        {

        }

        static void CreateManyCategory(SqlConnection connection)
        {
            var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Description = "Categoria de serviços do AWS";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;

            //

            var category2 = new Category();
            category2.Id = Guid.NewGuid();
            category2.Title = "Categoria nova";
            category2.Url = "Categoria-nova";
            category2.Description = "Categoria nova";
            category2.Order = 9;
            category2.Summary = "categoriaa";
            category2.Featured = true;

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

            var rows = connection.Execute(insertSql, new[]{
                new
                {
                    category.Id,
                    category.Title,
                    category.Url,
                    category.Summary,
                    category.Order,
                    category.Description,
                    category.Featured
                },
                new
                {
                    category.Id,
                    category2.Title,
                    category2.Url,
                    category2.Summary,
                    category2.Order,
                    category2.Description,
                    category2.Featured
                }
            });

            Console.WriteLine($"{rows} linhas inseridas");
        }

        static void UpdateCategory(SqlConnection connection)
        {
            var updateQuerry = "UPDATE [Category] SET [Title] =@title WHERE [Id]=@id";
            var rows = connection.Execute(updateQuerry, new
            {
                id = new Guid("af3407aa-11ae-4621-a2ef-2028b85507c4"),
                title = "Fronted 2021"
            });

            Console.WriteLine($"{rows} registros atualizados");
        }

        static void DeleteCategory(SqlConnection connection)
        {

        }

        static void ExecuteReadProcedure(SqlConnection connection)
        {
            var procedure = "[spGetCoursesByCategory]";
            var pars = new { CategoryId = "09ce0b7b-cfca-497b-92c0-3290ad9d5142" };
            var Courses = connection.Query(
                procedure,
                pars,
                commandType: CommandType.StoredProcedure);
            foreach (var c in Courses)
            {
                Console.WriteLine($"Id= {c.Id} titutlo: {c.Title}");

            }
        }

        static void ExecuteScalar(SqlConnection connection)
        {
            var category = new Category();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Description = "Categoria de serviços do AWS";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;

            // SQL Injection

            var insertSql = @"
                
                INSERT INTO 
                    [Category] 
                OUTPUT inserted.[Id]
                VALUES(
                    NEWID(),    
                    @Title,
                    @Url,
                    @Summary, 
                    @Order,  
                    @Description,
                    @Featured)
                ";

            var id = connection.ExecuteScalar<Guid>(insertSql,
                new
                {
                    category.Title,
                    category.Url,
                    category.Summary,
                    category.Order,
                    category.Description,
                    category.Featured
                });

            Console.WriteLine($"A categoria inserida foi: {id}");
        }

        static void ReadView(SqlConnection connection)
        {
            var sql = "SELECT * FROM [vwCourses]";

            var courses = connection.Query(sql);

            foreach (var item in courses)
            {
                Console.WriteLine($"{item.Id} - {item.Title}");
            }
        }

        static void OneToOne(SqlConnection connection)
        {
            var sql = @"
                SELECT 
                    * 
                FROM 
                    [CareerItem] 
                INNER JOIN 
                    [Course] ON [CareerItem].[CourseId] = [Course].[Id]";

            var items = connection.Query<CareerItem, Course, CareerItem>(
                sql,
                (careerItem, course) =>
                {
                    careerItem.Course = course;
                    return careerItem;
                }, splitOn: "Id");

            foreach (var item in items)
            {
                Console.WriteLine($"{item.Title} - Curso: {item.Course.Title}");
            }
        }

        static void OneToMany(SqlConnection connection)
        {
            var sql = @"
                    SELECT
                        [Career].[Id],
                        [Career].[Title],
                        [CareerItem].[CareerId] AS [Id],
                        [CareerItem].[CareerId]
                    FROM
                        [Career]
                    INNER JOIN  
                        [CareerItem] ON [CareerItem].[CareerId] = [Career].[Id]
                    ORDER BY 
                        [Career].[Title]
            ";
            var careers = new List<Career>();
            var items = connection.Query<Career, CareerItem, Career>(
                sql,
                (career, item) =>
                {
                    var car = careers.Where(x => x.Id == career.Id).FirstOrDefault();
                    if (car != null)
                    {
                        car = career;
                        car.Items.Add(item);
                        careers.Add(car);
                    }
                    else
                    {

                    }


                    career.Items.Add( item );
                    return career;
                }, splitOn: "CareerId");

            foreach (var career in careers)
            {
                Console.WriteLine($"{career.Title}");
                foreach (var item in career.Items)
                {
                    Console.WriteLine($" - {item.Title}");
                }
            }
        }


    }
}
