// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Testing;
using osu.Game.Database;
using osu.Game.Extensions;
using osu.Game.IO;
using osu.Game.Models;
using Realms;

#nullable enable

namespace osu.Game.Skinning
{
    [ExcludeFromDynamicCompile]
    [MapTo("Skin")]
    public class SkinInfo : RealmObject, IHasRealmFiles, IEquatable<SkinInfo>, IHasGuidPrimaryKey, ISoftDelete, IHasNamedFiles
    {
        internal static readonly Guid DEFAULT_SKIN = new Guid("2991CFD8-2140-469A-BCB9-2EC23FBCE4AD");
        internal static readonly Guid CLASSIC_SKIN = new Guid("81F02CD3-EEC6-4865-AC23-FAE26A386187");
        internal static readonly Guid RANDOM_SKIN = new Guid("D39DFEFB-477C-4372-B1EA-2BCEA5FB8908");

        [PrimaryKey]
        public Guid ID { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        public string Creator { get; set; } = string.Empty;

        public string Hash { get; set; } = string.Empty;

        public string InstantiationInfo { get; set; } = string.Empty;

        public virtual Skin CreateInstance(IStorageResourceProvider resources)
        {
            var type = string.IsNullOrEmpty(InstantiationInfo)
                // handle the case of skins imported before InstantiationInfo was added.
                ? typeof(LegacySkin)
                : Type.GetType(InstantiationInfo).AsNonNull();

            return (Skin)Activator.CreateInstance(type, this, resources);
        }

        public IList<RealmNamedFileUsage> Files { get; } = null!;

        public bool DeletePending { get; set; }

        public static SkinInfo Default { get; } = new SkinInfo
        {
            ID = DEFAULT_SKIN,
            Name = "osu! (triangles)",
            Creator = "team osu!",
            InstantiationInfo = typeof(DefaultSkin).GetInvariantInstantiationInfo()
        };

        public bool Equals(SkinInfo? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other == null) return false;

            return ID == other.ID;
        }

        public override string ToString()
        {
            string author = string.IsNullOrEmpty(Creator) ? string.Empty : $"({Creator})";
            return $"{Name} {author}".Trim();
        }

        IEnumerable<INamedFileUsage> IHasNamedFiles.Files => Files;
    }
}
