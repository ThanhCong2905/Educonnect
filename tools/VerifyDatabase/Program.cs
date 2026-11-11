using Microsoft.Extensions.Configuration;
using EduConnect.Mvc.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
await using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(configuration["ConnectionStrings:EduConnect"]).Options);
await db.Database.OpenConnectionAsync();
var missing = new List<string>();
foreach (var entity in db.Model.GetEntityTypes())
{
    var name = entity.GetTableName() ?? entity.GetViewName();
    if (name == null) continue;
    var schema = entity.GetSchema() ?? entity.GetViewSchema() ?? "dbo";
    var store = entity.GetTableName() != null ? StoreObjectIdentifier.Table(name,entity.GetSchema()) : StoreObjectIdentifier.View(name,entity.GetViewSchema());
    foreach (var property in entity.GetProperties())
    {
        var column = property.GetColumnName(store);
        if (column == null) continue;
        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM sys.columns WHERE object_id=OBJECT_ID(@object) AND name=@column";
        var objectParam=command.CreateParameter(); objectParam.ParameterName="@object"; objectParam.Value=schema+"."+name; command.Parameters.Add(objectParam);
        var columnParam=command.CreateParameter(); columnParam.ParameterName="@column"; columnParam.Value=column; command.Parameters.Add(columnParam);
        if (Convert.ToInt32(await command.ExecuteScalarAsync())==0) missing.Add(schema+"."+name+"."+column);
    }
}
foreach(var item in missing.Distinct()) Console.WriteLine("MISSING: "+item);
Console.WriteLine($"Checked {db.Model.GetEntityTypes().Count()} entity mappings; missing {missing.Distinct().Count()} columns.");
Environment.ExitCode=missing.Count==0?0:1;

if (missing.Count == 0)
{
    var accounts = await db.Set<EduConnect.Mvc.Models.NguoiThamGia>().AsNoTracking().Where(x=>x.LoaiNguoi!="QuanTri").ToListAsync();
    foreach(var account in accounts)
    {
        var http = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        http.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[] {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier,account.Id.ToString()),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role,account.LoaiNguoi)
        },"DatabaseVerification"));
        var current = new EduConnect.Mvc.Infrastructure.NguoiDungHienTaiService(db,new Microsoft.AspNetCore.Http.HttpContextAccessor { HttpContext=http });
        var notices = new EduConnect.Mvc.Infrastructure.ThongBaoService(db,current);
        var connections = new EduConnect.Mvc.Areas.KetNoi.Services.KetNoiService(db,current,notices);
        var classes = new EduConnect.Mvc.Areas.HocTap.Services.LopHocService(db,current,new EduConnect.Mvc.Infrastructure.QuyenLopHocService(db,current),notices,connections);
        var search = new EduConnect.Mvc.Areas.TimKiem.Services.TimKiemService(db,current);
        await search.GoiYAsync();
        await classes.LayAsync();
        await notices.ChuaDocAsync();
    }
    Console.WriteLine($"PASS: authenticated home queries for {accounts.Count} existing accounts; no data modified.");
}
