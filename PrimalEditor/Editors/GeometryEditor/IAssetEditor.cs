using PrimalEditor.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimalEditor.Editors
{
    interface IAssetEditor
    {
        Asset Asset { get; }

        void SetAsset(Asset asset);
    }
}
