using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kuku.Models
{
    public class SP_RecipeDetails
    {
        public string FileName { get; set; }
        public byte[] OriginalImageData { get; set; }
        public int RecipeId { get; set; }
        public string DescriptionRD { get; set; }
        public byte[] PreviewImageData { get; set; }
        public byte[] BigImageData { get; set; }

    }
}
/*CREATE procedure [dbo].[SP_RecipeDetails]
@FileName nvarchar(50),
@OriginalImageData varbinary(max),
@RecipeId int,
@DescriptionRD nvarchar(4000),
@PreviewImageData varbinary(max),
@BigImageData varbinary(max)
AS

INSERT INTO OriginalImage
(FileName, OriginalImageData)
Values (@FileName, @OriginalImageData);

INSERT INTO RecipeDetails
(RecipeId, Description, OriginalImageId, BigImageData, PreviewImageData)
Values (@RecipeId, @DescriptionRD, @@IDENTITY, @BigImageData, @PreviewImageData)
GO*/
