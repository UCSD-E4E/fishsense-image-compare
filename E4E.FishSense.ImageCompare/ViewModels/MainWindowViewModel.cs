using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;

namespace E4E.FishSense.ImageCompare.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private Bitmap? _leftImage;
    private CroppedBitmap? _leftImageCropped;
    private RectangleGeometry? _leftImageRectangle;
    private Uri? _leftImageUri;
    private int _leftCropPositionX;
    private int _leftCropPositionY;
    private int _leftCropWidth;
    private int _leftCropHeight;
    private Bitmap? _rightImage;
    private CroppedBitmap? _rightImageCropped;
    private Uri? _rightImageUri;
    private int _rightCropPositionX;
    private int _rightCropPositionY;
    private int _rightCropWidth;
    private int _rightCropHeight;
    private double _sliderValue;
    private double _zoom;

    public Bitmap? LeftImage
    {
        get => _leftImage;
        private set
        {
            SetProperty(ref _leftImage, value);
            if (value == null) return;
            
            LeftCropWidth = (int)value.Size.Width;
            LeftCropHeight = (int)value.Size.Height;
            LeftImageCropped = new CroppedBitmap(value, GetLeftImageCrop());
        }
    }

    public CroppedBitmap? LeftImageCropped
    {
        get => _leftImageCropped;
        set => SetProperty(ref _leftImageCropped, value);
    }

    public RectangleGeometry? LeftImageRectangle
    {
        get => _leftImageRectangle;
        set => SetProperty(ref _leftImageRectangle, value);
    }

    public Uri? LeftImageUri
    {
        get => _leftImageUri;
        set => SetProperty(ref _leftImageUri, value);
    }

    public int LeftCropPositionX
    {
        get => _leftCropPositionX;
        set
        {
            SetProperty(ref _leftCropPositionX, value);
            if (LeftImageCropped == null) return;
            LeftImageCropped.SourceRect = GetLeftImageCrop();
        }
    }

    public int LeftCropPositionY
    {
        get => _leftCropPositionY;
        set
        {
            SetProperty(ref _leftCropPositionY, value);
            if (LeftImageCropped == null) return;
            LeftImageCropped.SourceRect = GetLeftImageCrop();
        }
    }

    public int LeftCropWidth
    {
        get => _leftCropWidth;
        set
        {
            SetProperty(ref _leftCropWidth, value);
            if (LeftImageCropped == null) return;
            LeftImageCropped.SourceRect = GetLeftImageCrop();
        }
    }

    public int LeftCropHeight
    {
        get => _leftCropHeight;
        set
        {
            SetProperty(ref _leftCropHeight, value);
            if (LeftImageCropped == null) return;
            LeftImageCropped.SourceRect = GetLeftImageCrop();
        }
    }

    public Bitmap? RightImage
    {
        get => _rightImage;
        set
        {
            SetProperty(ref _rightImage, value);
            if (value == null) return;
            
            RightCropWidth = (int)value.Size.Width;
            RightCropHeight = (int)value.Size.Height;
            RightImageCropped = new CroppedBitmap(value, GetRightImageCrop());
        }
    }

    public CroppedBitmap? RightImageCropped
    {
        get => _rightImageCropped;
        set => SetProperty(ref _rightImageCropped, value);
    }

    public Uri? RightImageUri
    {
        get => _rightImageUri;
        set => SetProperty(ref _rightImageUri, value);
    }

    public int RightCropPositionX
    {
        get => _rightCropPositionX;
        set
        {
            SetProperty(ref _rightCropPositionX, value);
            if (RightImageCropped == null) return;
            RightImageCropped.SourceRect = GetRightImageCrop();
        }
    }

    public int RightCropPositionY
    {
        get => _rightCropPositionY;
        set
        {
            SetProperty(ref _rightCropPositionY, value);
            if (RightImageCropped == null) return;
            RightImageCropped.SourceRect = GetRightImageCrop();
        }
    }

    public int RightCropWidth
    {
        get => _rightCropWidth;
        set
        {
            SetProperty(ref _rightCropWidth, value);
            if (RightImageCropped == null) return;
            RightImageCropped.SourceRect = GetRightImageCrop();
        }
    }

    public int RightCropHeight
    {
        get => _rightCropHeight;
        set
        {
            SetProperty(ref _rightCropHeight, value);
            if (RightImageCropped == null) return;
            RightImageCropped.SourceRect = GetRightImageCrop();
        }
    }

    public double SliderValue
    {
        get => _sliderValue;
        set
        {
            SetProperty(ref _sliderValue, value);
            LeftImageRectangle = GetLeftImageRectangle();
        }
    }

    public double Zoom
    {
        get => _zoom;
        set => SetProperty(ref _zoom, value);
    }
    
    public ICommand BrowseLeftImageCommand { get; }
    public ICommand BrowseRightImageCommand { get; }

    public MainWindowViewModel()
    {
        BrowseLeftImageCommand = new AsyncRelayCommand<Visual>(BrowseLeftImageAsync);
        BrowseRightImageCommand = new AsyncRelayCommand<Visual>(BrowseRightImageAsync);
    }

    private async Task<Tuple<Uri?, Bitmap?>> BrowseImageAsync(Visual? visual)
    {
        var topLevel = TopLevel.GetTopLevel(visual);
        if (topLevel == null)  return  new Tuple<Uri?, Bitmap?>(null, null);
        
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            FileTypeFilter =  new List<FilePickerFileType>
            {
                FilePickerFileTypes.ImageAll
            },
        });

        if (files.Count < 1) return new Tuple<Uri?, Bitmap?>(null, null);
        var uri = files[0].Path;

        await using var stream = await files[0].OpenReadAsync();
        var bitmap = new Bitmap(stream);
            
        return new  Tuple<Uri?, Bitmap?>(uri, bitmap);

    }
    
    private async Task BrowseLeftImageAsync(Visual? visual)
    {
        var (uri, bitmap) = await BrowseImageAsync(visual);
        if (uri == null || bitmap == null) return;
        
        LeftImageUri = uri;
        LeftImage = bitmap;
    }
    
    private async Task BrowseRightImageAsync(Visual? visual)
    {
        var (uri, bitmap) = await BrowseImageAsync(visual);
        if (uri == null || bitmap == null) return;
        
        RightImageUri = uri;
        RightImage = bitmap;
        SliderValue = 0;
    }

    private PixelRect GetLeftImageCrop() =>
        new (LeftCropPositionX, LeftCropPositionY, LeftCropWidth, LeftCropHeight);
    
    private PixelRect GetRightImageCrop() =>
        new (RightCropPositionX, RightCropPositionY, RightCropWidth, RightCropHeight);

    private RectangleGeometry? GetLeftImageRectangle()
    {
        return LeftImage == null ? null : new RectangleGeometry(new Rect(0, 0, SliderValue, LeftImage.Size.Height));
    }
}