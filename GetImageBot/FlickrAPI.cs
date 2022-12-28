using FlickrNet;

public static class FlickrAPI
{
    private static readonly Flickr _flickr = new Flickr("c77b1b60abbb5751a82a01e545280c8d");
    private static readonly Random _random = new Random();
    
    
    public static async Task<string> GetPhotoUrlAsync(string request)
    {
        var photoSearchOptions = new PhotoSearchOptions
        {
            Text = request,
            SortOrder = PhotoSearchSortOrder.Relevance
        };
        
        PhotoCollection photos = await _flickr.PhotosSearchAsync(photoSearchOptions);
        var listPhotos = photos.ToList();

        var randomPhotos = _random.Next(0, listPhotos.Count);
        return listPhotos[randomPhotos].LargeUrl;
    }
}