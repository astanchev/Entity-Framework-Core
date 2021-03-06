﻿namespace MusicHub.DataProcessor.ExportDtos
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlType("Song")]
    public class SongExportXmlDto2
    {
        [XmlElement(ElementName="SongName")]
        public string SongName { get; set; }

        [XmlElement(ElementName="Writer")]
        public string Writer { get; set; }

        [XmlElement(ElementName="Performer")]
        public List<string> Performers { get; set; }

        [XmlElement(ElementName="AlbumProducer")]
        public string AlbumProducer { get; set; }

        [XmlElement(ElementName="Duration")]
        public string Duration { get; set; }
    }
}