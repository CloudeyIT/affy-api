using System.Threading.Tasks;
using Affy.Core.Framework.Database;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Affy.Test;

[SetUpFixture]
public class DatabaseSetup : IntegrationFixture
{
    [OneTimeSetUp]
    public async Task RunMigrations ()
    {
        await Database.Database.MigrateAsync();
    }

    [OneTimeTearDown]
    public async Task CleanDatabase ()
    {
        await Database.DropAllTables();
    }
}