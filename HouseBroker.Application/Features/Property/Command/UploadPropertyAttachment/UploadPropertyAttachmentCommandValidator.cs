using FluentValidation;
using HouseBroker.Application.Features.Property.Command.UploadPropertyAttachment;

namespace HouseBroker.Application.Features.Property.Command.UploadPropertyImage;

public class UploadPropertyAttachmentCommandValidator : AbstractValidator<UploadPropertyAttachmentCommand>
{
    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png" };
    private const long MaxImageSizeBytes = 5 * 1024 * 1024; // 5MB

    public UploadPropertyAttachmentCommandValidator()
    {
        // PropertyId validation
        RuleFor(x => x.PropertyId)
            .NotEmpty()
            .WithMessage("Property ID is required")
            .NotEqual(Guid.Empty)
            .WithMessage("Property ID cannot be empty");
        
        // FileName validation
        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("File name is required")
            .MaximumLength(255)
            .WithMessage("File name cannot exceed 255 characters")
            .Must(HaveValidImageExtension)
            .WithMessage($"Only image files are allowed: {string.Join(", ", AllowedImageExtensions)}")
            .Must(NotContainInvalidCharacters)
            .WithMessage("File name contains invalid characters");
        
        RuleFor(x => x.FileContent)
            .NotNull()
            .WithMessage("Image file is required")
            .Must(BeReadableStream)
            .WithMessage("Image file must be readable")
            .Must(HaveValidImageSize)
            .WithMessage($"Image size cannot exceed {MaxImageSizeBytes / (1024 * 1024)}MB")
            .Must(BeValidImageFile)
            .WithMessage("File is not a valid image or is corrupted");

    }
    private static bool HaveValidImageExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return false;
        
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedImageExtensions.Contains(extension);
    }
    private static bool NotContainInvalidCharacters(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return false;
        
        var invalidChars = Path.GetInvalidFileNameChars();
        return !fileName.Any(c => invalidChars.Contains(c));
    }
    private static bool BeReadableStream(Stream? stream)
    {
        return stream is { CanRead: true };
    }
    
    private static bool HaveValidImageSize(Stream? stream)
    {
        if (stream == null) return false;
        
        try
        {
            return stream.Length > 0 && stream.Length <= MaxImageSizeBytes;
        }
        catch
        {
            return false;
        }
    }
    
    private static bool BeValidImageFile(UploadPropertyAttachmentCommand command, Stream stream)
    {
        if (stream == null || string.IsNullOrEmpty(command.FileName)) return false;
        
        try
        {
            var extension = Path.GetExtension(command.FileName).ToLowerInvariant();
            
            // Reset stream position for reading
            var originalPosition = stream.Position;
            stream.Position = 0;
            
            var buffer = new byte[512];
            var bytesRead = stream.Read(buffer, 0, buffer.Length);
            
            // Reset stream position
            stream.Position = originalPosition;
            
            if (bytesRead < 4) return false;
            
            // Check image file signatures (magic numbers)
            return extension switch
            {
                ".jpg" or ".jpeg" => IsJpeg(buffer),
                ".png" => IsPng(buffer),
                _ => false
            };
        }
        catch
        {
            return false;
        }
    }
    private static bool IsJpeg(byte[] buffer)
    {
        return buffer.Length >= 3 && 
               buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF;
    }

    private static bool IsPng(byte[] buffer)
    {
        return buffer.Length >= 8 && 
               buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47 &&
               buffer[4] == 0x0D && buffer[5] == 0x0A && buffer[6] == 0x1A && buffer[7] == 0x0A;
    }
}