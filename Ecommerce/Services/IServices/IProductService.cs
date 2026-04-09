namespace Ecommerce.Services.IServices
{
    public interface IProductService
    {
        Task<string> CreateFileAsync(IFormFile Img, ProductImgType productImgType = ProductImgType.MainImg);
        string GetOldFilePath(string oldFileName, ProductImgType productImgType = ProductImgType.MainImg);
    }
}
