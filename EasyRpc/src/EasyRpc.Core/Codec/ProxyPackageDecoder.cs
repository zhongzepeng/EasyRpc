﻿using System;

namespace EasyRpc.Core.Codec
{
    public class ProxyPackageDecoder : IPackageDecoder<ProxyPackage>
    {
        public ProxyPackage Decode(ReadOnlySpan<byte> span)
        {
            var pack = new ProxyPackage();
            var pos = 0;

            var framelength = span.Slice(pos, ProxyPackage.HEADER_SIZE).ToInt();
            pos += ProxyPackage.HEADER_SIZE;

            pack.Type = (PackageType)span.Slice(pos, ProxyPackage.TYPE_SIZE)[0];
            pos += ProxyPackage.TYPE_SIZE;

            pack.ConnectionId = span.Slice(pos, ProxyPackage.CONNECTIONID_SIZE).ToLong();

            pos += ProxyPackage.CONNECTIONID_SIZE;
            pack.ChannelId = span.Slice(pos, ProxyPackage.CHANNELID_SIZE).ToInt();

            pos += ProxyPackage.CHANNELID_SIZE;
            pack.Data = span.Slice(pos).ToArray();
            return pack;
        }
    }
}
