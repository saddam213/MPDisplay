// <copyright file="PropertyTagId.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using MediaPortalPlugin.ExifReader.PropertyFormatters;
using MediaPortalPlugin.ExifReader.UndefinedExtractor;

namespace MediaPortalPlugin.ExifReader
{

    /// <summary>
    /// Defines the common Exif property Ids
    /// </summary>
    /// <remarks>
    /// This is not a comprehensive list since there are several non-standard Ids in use (example those from Adobe)
    /// </remarks>
    public enum PropertyTagId
    {
        GpsVer = 0x0000,

        GpsLatitudeRef = 0x0001,

        [ExifPropertyFormatter(typeof(GpsLatitudeLongitudePropertyFormatter), ConstructorNeedsPropertyTag = true)]
        [EnumDisplayName("GPS Latitude")]
        GpsLatitude = 0x0002,

        GpsLongitudeRef = 0x0003,

        [ExifPropertyFormatter(typeof(GpsLatitudeLongitudePropertyFormatter), ConstructorNeedsPropertyTag = true)]
        [EnumDisplayName("GPS Longitude")]
        GpsLongitude = 0x0004,

        GpsAltitudeRef = 0x0005,

        GpsAltitude = 0x0006,

        [ExifPropertyFormatter(typeof(GpsTimePropertyFormatter))]
        GpsGpsTime = 0x0007,

        GpsGpsSatellites = 0x0008,

        GpsGpsStatus = 0x0009,

        GpsGpsMeasureMode = 0x000A,

        GpsGpsDop = 0x000B,

        GpsSpeedRef = 0x000C,

        GpsSpeed = 0x000D,

        GpsTrackRef = 0x000E,

        GpsTrack = 0x000F,

        GpsImgDirRef = 0x0010,

        GpsImgDir = 0x0011,

        GpsMapDatum = 0x0012,

        GpsDestLatRef = 0x0013,

        GpsDestLat = 0x0014,

        GpsDestLongRef = 0x0015,

        GpsDestLong = 0x0016,

        GpsDestBearRef = 0x0017,

        GpsDestBear = 0x0018,

        GpsDestDistRef = 0x0019,

        GpsDestDist = 0x001A,

        NewSubfileType = 0x00FE,

        SubfileType = 0x00FF,

        ImageWidth = 0x0100,

        ImageHeight = 0x0101,

        BitsPerSample = 0x0102,

        Compression = 0x0103,

        PhotometricInterp = 0x0106,

        ThreshHolding = 0x0107,

        CellWidth = 0x0108,

        CellHeight = 0x0109,

        FillOrder = 0x010A,

        DocumentName = 0x010D,

        ImageDescription = 0x010E,

        EquipMake = 0x010F,

        EquipModel = 0x0110,

        StripOffsets = 0x0111,

        Orientation = 0x0112,

        SamplesPerPixel = 0x0115,

        RowsPerStrip = 0x0116,

        StripBytesCount = 0x0117,

        MinSampleValue = 0x0118,

        MaxSampleValue = 0x0119,

        [ExifPropertyFormatter(typeof(GenericRational32PropertyFormatter), ConstructorNeedsPropertyTag = true)]
        [EnumDisplayName("Horizontal resolution")]
        XResolution = 0x011A,
        
        [ExifPropertyFormatter(typeof(GenericRational32PropertyFormatter), ConstructorNeedsPropertyTag = true)]
        [EnumDisplayName("Vertical resolution")]
        YResolution = 0x011B,

        PlanarConfig = 0x011C,

        PageName = 0x011D,

        XPosition = 0x011E,

        YPosition = 0x011F,

        FreeOffset = 0x0120,

        FreeByteCounts = 0x0121,

        GrayResponseUnit = 0x0122,

        GrayResponseCurve = 0x0123,

        T4Option = 0x0124,

        T6Option = 0x0125,

        [ExifPropertyFormatter(typeof(ResolutionUnitPropertyFormatter))]
        ResolutionUnit = 0x0128,

        PageNumber = 0x0129,

        TransferFunction = 0x012D,

        SoftwareUsed = 0x0131,

        DateTime = 0x0132,

        Artist = 0x013B,

        HostComputer = 0x013C,

        Predictor = 0x013D,

        WhitePoint = 0x013E,

        PrimaryChromaticities = 0x013F,

        ColorMap = 0x0140,

        HalftoneHints = 0x0141,

        TileWidth = 0x0142,

        TileLength = 0x0143,

        TileOffset = 0x0144,

        TileByteCounts = 0x0145,

        InkSet = 0x014C,

        InkNames = 0x014D,

        NumberOfInks = 0x014E,

        DotRange = 0x0150,

        TargetPrinter = 0x0151,

        ExtraSamples = 0x0152,

        SampleFormat = 0x0153,

        SMinSampleValue = 0x0154,

        SMaxSampleValue = 0x0155,

        TransferRange = 0x0156,

        JPEG_PROC = 0x0200,

        JPEG_INTER_FORMAT = 0x0201,

        JPEG_INTER_LENGTH = 0x0202,

        JPEG_RESTART_INTERVAL = 0x0203,

        JPEG_LOSSLESS_PREDICTORS = 0x0205,

        JPEG_POINT_TRANSFORMS = 0x0206,

        JPEGQ_TABLES = 0x0207,

        JPEGDC_TABLES = 0x0208,

        JPEGAC_TABLES = 0x0209,

        YCbCrCoefficients = 0x0211,

        YCbCrSubsampling = 0x0212,

        YCbCrPositioning = 0x0213,

        REF_BLACK_WHITE = 0x0214,

        Gamma = 0x0301,

        ICC_PROFILE_DESCRIPTOR = 0x0302,

        SRGB_RENDERING_INTENT = 0x0303,

        ImageTitle = 0x0320,

        ResolutionXUnit = 0x5001,

        ResolutionYUnit = 0x5002,

        ResolutionXLengthUnit = 0x5003,

        ResolutionYLengthUnit = 0x5004,

        PrintFlags = 0x5005,

        PrintFlagsVersion = 0x5006,

        PrintFlagsCrop = 0x5007,

        PrintFlagsBleedWidth = 0x5008,

        PrintFlagsBleedWidthScale = 0x5009,

        HALFTONE_LPI = 0x500A,

        HALFTONE_LPI_UNIT = 0x500B,

        HalftoneDegree = 0x500C,

        HalftoneShape = 0x500D,

        HalftoneMisc = 0x500E,

        HalftoneScreen = 0x500F,

        JPEG_QUALITY = 0x5010,

        GridSize = 0x5011,

        ThumbnailFormat = 0x5012,

        ThumbnailWidth = 0x5013,

        ThumbnailHeight = 0x5014,

        ThumbnailColorDepth = 0x5015,

        ThumbnailPlanes = 0x5016,

        ThumbnailRawBytes = 0x5017,

        ThumbnailSize = 0x5018,

        ThumbnailCompressedSize = 0x5019,

        ColorTransferFunction = 0x501A,

        ThumbnailData = 0x501B,

        ThumbnailImageWidth = 0x5020,

        ThumbnailImageHeight = 0x5021,

        ThumbnailBitsPerSample = 0x5022,

        ThumbnailCompression = 0x5023,

        ThumbnailPhotometricInterp = 0x5024,

        ThumbnailImageDescription = 0x5025,

        ThumbnailEquipMake = 0x5026,

        ThumbnailEquipModel = 0x5027,

        ThumbnailStripOffsets = 0x5028,

        ThumbnailOrientation = 0x5029,

        ThumbnailSamplesPerPixel = 0x502A,

        ThumbnailRowsPerStrip = 0x502B,

        ThumbnailStripBytesCount = 0x502C,

        [ExifPropertyFormatter(typeof(GenericRational32PropertyFormatter), ConstructorNeedsPropertyTag = true)]
        ThumbnailResolutionX = 0x502D,

        [ExifPropertyFormatter(typeof(GenericRational32PropertyFormatter), ConstructorNeedsPropertyTag = true)]
        ThumbnailResolutionY = 0x502E,

        ThumbnailPlanarConfig = 0x502F,

        ThumbnailResolutionUnit = 0x5030,

        ThumbnailTransferFunction = 0x5031,

        ThumbnailSoftwareUsed = 0x5032,

        ThumbnailDateTime = 0x5033,

        ThumbnailArtist = 0x5034,

        ThumbnailWhitePoint = 0x5035,

        ThumbnailPrimaryChromaticities = 0x5036,

        ThumbnailYCbCrCoefficients = 0x5037,

        ThumbnailYCbCrSubsampling = 0x5038,

        ThumbnailYCbCrPositioning = 0x5039,

        ThumbnailRefBlackWhite = 0x503A,

        ThumbnailCopyRight = 0x503B,

        LuminanceTable = 0x5090,

        ChrominanceTable = 0x5091,

        FrameDelay = 0x5100,

        LoopCount = 0x5101,

        GlobalPalette = 0x5102,

        IndexBackground = 0x5103,

        IndexTransparent = 0x5104,

        PixelUnit = 0x5110,

        PixelPerUnitX = 0x5111,

        PixelPerUnitY = 0x5112,

        PaletteHistogram = 0x5113,

        Copyright = 0x8298,

        [ExifPropertyFormatter(typeof(ExifExposureTimePropertyFormatter))]
        ExifExposureTime = 0x829A,

        [ExifPropertyFormatter(typeof(ExifFNumberPropertyFormatter), ConstructorNeedsPropertyTag = true)]
        [EnumDisplayName("F-Stop")]
        ExifFNumber = 0x829D,

        EXIF_IFD = 0x8769,

        ICC_PROFILE = 0x8773,

        [ExifPropertyFormatter(typeof(ExifExposureProgPropertyFormatter))]
        ExifExposureProg = 0x8822,

        ExifSpectralSense = 0x8824,

        GPS_IFD = 0x8825,

        [ExifPropertyFormatter(typeof(ExifISOSpeedPropertyFormatter))]
        ExifISOSpeed = 0x8827,

        EXIF_OECF = 0x8828,

        [ExifValueUndefinedExtractor(typeof(GenericStringUndefinedExtractor))]
        ExifVer = 0x9000,

        EXIF_DT_ORIG = 0x9003,

        EXIF_DT_DIGITIZED = 0x9004,

        ExifCompConfig = 0x9101,

        [ExifPropertyFormatter(typeof(GenericRational32PropertyFormatter), ConstructorNeedsPropertyTag = true)]
        [EnumDisplayName("Compressed Bits Per Pixel")]
        EXIF_COMP_BPP = 0x9102,

        [ExifPropertyFormatter(typeof(ExifShutterSpeedPropertyFormatter))]
        ExifShutterSpeed = 0x9201,

        [ExifPropertyFormatter(typeof(ExifFNumberPropertyFormatter), ConstructorNeedsPropertyTag = true)]
        [EnumDisplayName("Exif Aperture")]
        ExifAperture = 0x9202,

        ExifBrightness = 0x9203,

        [ExifPropertyFormatter(typeof(ExifExposureBiasPropertyFormatter))]
        ExifExposureBias = 0x9204,

        [ExifPropertyFormatter(typeof(ExifFNumberPropertyFormatter), ConstructorNeedsPropertyTag = true)]
        [EnumDisplayName("Exif Max Aperture")]
        ExifMaxAperture = 0x9205,

        [ExifPropertyFormatter(typeof(ExifSubjectDistPropertyFormatter), ConstructorNeedsPropertyTag = true)]
        [EnumDisplayName("Subject Distance")]
        ExifSubjectDist = 0x9206,

        [ExifPropertyFormatter(typeof(ExifMeteringModePropertyFormatter))]
        ExifMeteringMode = 0x9207,

        [ExifPropertyFormatter(typeof(ExifLightSourcePropertyFormatter))]
        ExifLightSource = 0x9208,

        [ExifPropertyFormatter(typeof(ExifFlashPropertyFormatter))]
        ExifFlash = 0x9209,

        [ExifPropertyFormatter(typeof(ExifFocalLengthPropertyFormatter), ConstructorNeedsPropertyTag = true)]
        [EnumDisplayName("Focal Length")]
        ExifFocalLength = 0x920A,

        ExifMakerNote = 0x927C,

        ExifUserComment = 0x9286,

        EXIF_DT_SUBSEC = 0x9290,

        EXIF_DT_ORIG_SS = 0x9291,

        EXIF_DT_DIG_SS = 0x9292,

        [ExifValueUndefinedExtractor(typeof(GenericStringUndefinedExtractor))]
        EXIF_FPX_VER = 0xA000,

        [ExifPropertyFormatter(typeof(ExifColorSpacePropertyFormatter))]
        ExifColorSpace = 0xA001,

        ExifPixXDim = 0xA002,

        ExifPixYDim = 0xA003,

        ExifRelatedWav = 0xA004,

        ExifInterop = 0xA005,

        ExifFlashEnergy = 0xA20B,

        EXIF_SPATIAL_FR = 0xA20C,

        [ExifPropertyFormatter(typeof(GenericRational32PropertyFormatter), ConstructorNeedsPropertyTag = true)]
        ExifFocalXRes = 0xA20E,

        [ExifPropertyFormatter(typeof(GenericRational32PropertyFormatter), ConstructorNeedsPropertyTag = true)]
        ExifFocalYRes = 0xA20F,

        [ExifPropertyFormatter(typeof(ExifFocalResUnitPropertyFormatter))]
        ExifFocalResUnit = 0xA210,

        ExifSubjectLoc = 0xA214,

        ExifExposureIndex = 0xA215,

        [ExifPropertyFormatter(typeof(ExifSensingMethodPropertyFormatter))]
        ExifSensingMethod = 0xA217,

        [ExifValueUndefinedExtractor(typeof(ExifFileSourceUndefinedExtractor))]
        [EnumDisplayName("File Source")]
        ExifFileSource = 0xA300,

        [ExifValueUndefinedExtractor(typeof(ExifSceneTypeUndefinedExtractor))]
        [EnumDisplayName("Scene Type")]
        ExifSceneType = 0xA301,

        ExifCfaPattern = 0xA302,

        // New Exif Tag Ids
        ExifCustomRendered = 0xA401,

        [ExifPropertyFormatter(typeof(ExifExposureModePropertyFormatter))]
        ExifExposureMode = 0xA402,

        [ExifPropertyFormatter(typeof(WhiteBalancePropertyFormatter))]
        ExifWhiteBalance = 0xA403,

        [ExifPropertyFormatter(typeof(GenericRational32PropertyFormatter), ConstructorNeedsPropertyTag = true)]
        [EnumDisplayName("Digital Zoom Ratio")]
        ExifDigitalZoomRatio = 0xA404,

        [EnumDisplayName("Focal Length (35 mm)")]
        EXIF_FOCAL_LENGTH_IN35_MM_FILM = 0xA405,

        [ExifPropertyFormatter(typeof(ExifSceneCaptureTypePropertyFormatter))]
        ExifSceneCaptureType = 0xA406,

        [ExifPropertyFormatter(typeof(ExifGainControlPropertyFormatter))]
        ExifGainControl = 0xA407,

        [ExifPropertyFormatter(typeof(ExifContrastPropertyFormatter))]
        ExifContrast = 0xA408,

        [ExifPropertyFormatter(typeof(ExifSaturationPropertyFormatter))]
        ExifSaturation = 0xA409,

        [ExifPropertyFormatter(typeof(ExifSharpnessPropertyFormatter))]
        ExifSharpness = 0xA40A,

        ExifDeviceSettingDesc = 0xA40B,

        [ExifPropertyFormatter(typeof(ExifSubjectDistanceRangePropertyFormatter))]
        ExifSubjectDistanceRange = 0xA40C,

        EXIF_UNIQUE_IMAGE_ID = 0xA420,

        [ExifPropertyFormatter(typeof(GenericRational32PropertyFormatter), ConstructorNeedsPropertyTag = true)]
        ExifGamma = 0xA500,
        
        [EnumDisplayName("Interoperability Index")]
        ExifInteroperabilityIndex = 0x5041,

        [ExifValueUndefinedExtractor(typeof(GenericStringUndefinedExtractor))]
        [EnumDisplayName("Interoperability Version")]
        ExifInteroperabilityVersion = 0x5042,

        [EnumDisplayName("Related Image Width")]
        ExifRelatedImageWidth = 0x1001,

        [EnumDisplayName("Related Image Height")]
        ExifRelatedImageHeight = 0x1002,

        [EnumDisplayName("Unknown Exif Tag")]
        UnknownExifTag = 0xFFFF
    }
}