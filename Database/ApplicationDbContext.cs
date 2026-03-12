using System;
using Microsoft.EntityFrameworkCore;

namespace wealthify.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{

}
