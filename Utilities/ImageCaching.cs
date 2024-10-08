﻿using System.Drawing;
using Boxy_Core.Model;

namespace Boxy_Core.Utilities
{
    /// <summary>
    /// Class for accessing bitmap objects pulled from the API, to avoid re-querying for them.
    /// </summary>
    public static class ImageCaching
    {
        private static Dictionary<string, Bitmap>? _imageCache;

        private static Dictionary<string, Bitmap> ImageCache
        {
            get
            {
                return _imageCache ??= new Dictionary<string, Bitmap>();
            }
        }

        private static bool IsCacheBeingAccessed { get; set; }

        /// <summary>
        /// Gets the cached bitmap image representing the card object. Will query the API if it has not been loaded, otherwise gets the cached version.
        /// </summary>
        public static async Task<Bitmap?> GetImageAsync(ScryfallService scryfallService, string imageUri, IProgress<string> reporter)
        {
            if (string.IsNullOrWhiteSpace(imageUri))
            {
                throw new ArgumentNullException(nameof(imageUri), "Image request URI cannot be null or empty/whitespace. Consumer must check before using this method.");
            }

            while (IsCacheBeingAccessed)
            {
                await Task.Delay(1);
            }

            IsCacheBeingAccessed = true;

            if (ImageCache.ContainsKey(imageUri))
            {
                await Task.Delay(1);
                IsCacheBeingAccessed = false;
                return ImageCache[imageUri];
            }

            Bitmap? bitmap = await scryfallService.GetImageAsync(imageUri, reporter);

            if (bitmap is null)
            {
                return null;
            }

            ImageCache.Add(imageUri, bitmap);

            if (ImageCache.Count > 100)
            {
                ImageCache.Remove(ImageCache.First().Key);
            }

            IsCacheBeingAccessed = false;
            return bitmap;
        }

        /// <summary>
        /// Clears the cache to ensure the images are released from memory correctly.
        /// </summary>
        public static void Clear()
        {
            ImageCache.Clear();
        }
    }
}
