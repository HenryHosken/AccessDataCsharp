﻿using BaltaDataAccess.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BaltaDataAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            const string connectionString = "Server=localhost, 1433;Database=balta;User ID=sa;Password=H5nry@Hosken!S5norio;Trusted_Connection=False; TrustServerCertificate=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                //UpdateCategory(connection);
                //CreateManyCategory(connection);
                //ListCategories(connection);
                //DeleteCategory(connection);
                //GetCategory(connection);
                //ExecuteReadProcedure(connection);
                ExecuteScalar(connection);
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
    }
}
