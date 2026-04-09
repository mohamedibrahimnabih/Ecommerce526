using Ecommerce.Services.IServices;

namespace Ecommerce.Services
{
    public enum ProductImgType
    {
        MainImg,
        SubImg
    }

    public class ProductService : IProductService
    {
        public async Task<string> CreateFileAsync(IFormFile Img, ProductImgType productImgType = ProductImgType.MainImg)
        {
            var fileName =
                    $"{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}-{Guid.NewGuid().ToString()}{Path.GetExtension(Img.FileName)}";
            // 31290-fjkdsfhsd-32131.png

            var filePath = string.Empty;

            if (productImgType == ProductImgType.MainImg)
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products", fileName);
            }
            else if(productImgType == ProductImgType.SubImg)
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products\\SubImgs", fileName);
            }

            using (var stream = System.IO.File.Create(filePath))
            {
                await Img.CopyToAsync(stream);
            }

            return fileName;
        }

        public string GetOldFilePath(string oldFileName, ProductImgType productImgType = ProductImgType.MainImg)
        {
            var filePath = string.Empty;

            if (productImgType == ProductImgType.MainImg)
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products", oldFileName);
            }
            else if (productImgType == ProductImgType.SubImg)
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products\\SubImgs", oldFileName);
            }

            return filePath;
        }
    }
}
