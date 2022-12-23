using FlickrNet;

public class FlickrAPI
{
    private readonly Flickr _flickr;
    private readonly Random _random = new Random();

    public FlickrAPI(string apiKey)
    {
        _flickr = new Flickr(apiKey);
    }
    
    public async Task<string> GetPhotoUrlAsync(string request)
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