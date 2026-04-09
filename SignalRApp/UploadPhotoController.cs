using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalRApp.MessageFolder;
using SignalRApp.Services;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRApp
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photosAvaUsersFolder");

        [HttpPost("photosAvaUser")]
        public async Task<IActionResult> Upload(IFormFile file, [FromHeader] string Nickname)
        {
            if (file == null || file.Length == 0) return BadRequest();

            // Генерируем уникальное имя, чтобы избежать перезаписи
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_storagePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Возвращаем URL файла, который клиент сохранит в базу через SignalR
            var fileUrl = $"/photosAvaUsersFolder/{fileName}";

            await UpdateUrl(fileUrl, Nickname);

            return Ok(new { url = fileUrl });
        }
        private readonly DataBaze context;

        public FilesController(DataBaze context)
        {
            this.context = context;

        }

        private async Task UpdateUrl(string url, string Nickname)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Nickname == Nickname);

            if (user != null)
            {
                user.PhotoUser = url;
                await context.SaveChangesAsync();
            }
        }




        [HttpPost("Chat_{id}")]
        public async Task<IActionResult> AddPhotoChat(string id, IFormFile file)
        {
            string storagePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/Chat_{id}");

            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);

            if (file == null || file.Length == 0)
                return BadRequest();

            var FileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(storagePath, FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var FileURL = $"/Chat_{id}/{FileName}";
            //await AddPhotoChat_DB(id, );
            return Ok(new FileDto { FileURL = FileURL, FileName = FileName });
        }

        //private async Task AddPhotoChat_DB(string url, string Nickname)
        //{
        //    ChatHub chatHub = new ChatHub(context);
        //    chatHub.SendImageMessageUser()
        //    var user = await context.Users
        //        .FirstOrDefaultAsync(u => u.Nickname == Nickname);

        //    if (user != null)
        //    {
        //        user.PhotoUser = url;
        //        await context.SaveChangesAsync();
        //    }
        //}


    }


}
