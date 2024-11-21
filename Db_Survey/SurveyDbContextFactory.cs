using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Design;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace TestEntitySurvey
{
    public class SurveyDbContextFactory : IDesignTimeDbContextFactory<SurveyDbContext>
    {
        public SurveyDbContext CreateDbContext(string[]? args)
        {
          var config =  new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("Upsettings.json")
              .Build();


            var option = new DbContextOptionsBuilder<SurveyDbContext>()
                .UseSqlServer(config.GetConnectionString("SqlClient"))
                .Options;

            return new SurveyDbContext(option);


        }
    }
}
