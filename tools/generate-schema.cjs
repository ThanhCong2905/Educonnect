// Reproducible mapping of DB02, without modifying the source SQL or database.
const fs = require('node:fs');
const path = require('node:path');
const base = path.resolve(__dirname, '..');
const sql = fs.readFileSync(path.join(base, 'Database/00_Original_DB02.sql'), 'utf8');
let entities = 'using System.ComponentModel.DataAnnotations;\nnamespace EduConnect.Mvc.Models;\n';
let mapping = '';
for (const t of sql.matchAll(/CREATE TABLE dbo\.\[(\w+)\] \(([\s\S]*?)\n\);/g)) {
 const [,name,body] = t;
 const pk = body.match(/PRIMARY KEY \(([^)]+)\)/)[1].match(/\w+/g);
 entities += `public sealed class ${name} {\n`;
 mapping += `b.Entity<${name}>(e => { e.ToTable("${name}", t => t.UseSqlOutputClause(false)); e.HasKey(x => new { ${pk.map(k=>'x.'+k).join(', ')} });\n`;
 for (const c of body.matchAll(/^\s+\[(\w+)\] (\w+)(\([^)]+\))?( IDENTITY\(1,1\))? (NOT NULL|NULL)/gm)) {
  const [,col,type,size,identity,nullable] = c;
  const optional = nullable === 'NULL';
  const types = {int:'int',tinyint:'byte',bigint:'long',bit:'bool',uniqueidentifier:'Guid',decimal:'decimal',datetimeoffset:'DateTimeOffset',time:'TimeOnly',rowversion:'byte[]',nvarchar:'string',varchar:'string'};
  const cs = types[type]; if (!cs) throw Error(type);
  entities += ` public ${cs}${optional?'?':''} ${col} { get; set; }${!optional&&cs==='string'?' = "";':!optional&&cs==='byte[]'?' = [];':''}\n`;
  if (type === 'rowversion') mapping += `e.Property(x=>x.${col}).IsRowVersion();\n`;
  else {
   mapping += `e.Property(x=>x.${col}).HasColumnType("${type}${size||''}")${!optional?'.IsRequired()':''}${identity?'.ValueGeneratedOnAdd()':col==='Id'&&type==='uniqueidentifier'?'.ValueGeneratedNever()':''};\n`;
  }
 }
 entities += '}\n'; mapping += '});\n';
}
fs.writeFileSync(path.join(base,'Models/Entities.cs'),entities);
fs.writeFileSync(path.join(base,'Data/AppDbContext.cs'),`using EduConnect.Mvc.Models;\nusing Microsoft.EntityFrameworkCore;\nnamespace EduConnect.Mvc.Data;\npublic sealed class AppDbContext(DbContextOptions<AppDbContext> options): DbContext(options) {\nprotected override void OnModelCreating(ModelBuilder b) {\n${mapping}}\n}\n`);
// Retain all procedure checks and transactions; only change the approved learner permission.
const proc = sql.slice(sql.indexOf('CREATE OR ALTER PROCEDURE dbo.usp_ChapNhanDeXuatTaoLop'), sql.lastIndexOf('SELECT DB_NAME()'));
fs.writeFileSync(path.join(base,'Database/01_MainLearnerPermission.sql'),`SET QUOTED_IDENTIFIER ON;\nGO\nUSE [KLTN_4NghiepVu_V2];\nGO\n${proc.replace("(@LopId,@BenHocId,@GiaSuId,'DaDuyet','Xem'", "(@LopId,@BenHocId,@GiaSuId,'DaDuyet','ChinhSua'")}\n-- Approved change only for the principal learner, not additional members.\nUPDATE m SET MucQuyen='ChinhSua' FROM dbo.ThanhVienLopHoc m JOIN dbo.LopHoc c ON c.Id=m.LopHocId AND c.BenHocId=m.TaiKhoanId WHERE m.TrangThaiDuyet='DaDuyet' AND m.RoiLopLuc IS NULL AND m.MucQuyen='Xem';\n-- Original seed has no membership rows; repair only absent principal memberships.\nINSERT dbo.ThanhVienLopHoc(LopHocId,TaiKhoanId,NguoiDeNghiId,TrangThaiDuyet,MucQuyen,DuyetLuc) SELECT c.Id,p.Id,c.GiaSuId,'DaDuyet','ChinhSua',SYSDATETIMEOFFSET() FROM dbo.LopHoc c CROSS APPLY(VALUES(c.GiaSuId),(c.BenHocId)) p(Id) WHERE NOT EXISTS(SELECT 1 FROM dbo.ThanhVienLopHoc m WHERE m.LopHocId=c.Id AND m.TaiKhoanId=p.Id AND m.RoiLopLuc IS NULL AND m.TrangThaiDuyet<>'TuChoi');\n`);

