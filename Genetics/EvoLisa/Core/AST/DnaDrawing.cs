﻿using System.Collections.Generic;
using System.Xml.Serialization;
using GenArt.Classes;
using System;
using GenArt.Core.Classes;

namespace GenArt.AST
{
    [Serializable]
    public class DnaDrawing
    {
        public List<DnaPolygon> Polygons { get; set; }
        [XmlIgnore]
        public SourceImage SourceImage { get; set; }        

        [XmlIgnore]
        private bool IsDirty { get; set; }

        public int PointCount
        {
            get
            {
                int pointCount = 0;
                foreach (DnaPolygon polygon in Polygons)
                    pointCount += polygon.Points.Count;

                return pointCount;
            }
        }

        public void SetDirty()
        {
            IsDirty = true;
        }

        public void Init()
        {
            Polygons = new List<DnaPolygon>();

            for (int i = 0; i < Settings.ActivePolygonsMin; i++)
                AddPolygon();

            SetDirty();
        }

        public DnaDrawing Clone()
        {
            var drawing = new DnaDrawing
                              {
                                  Polygons = new List<DnaPolygon>(),
                                  SourceImage = SourceImage,
                              };
            foreach (DnaPolygon polygon in Polygons)
                drawing.Polygons.Add(polygon.Clone());
            return drawing;
        }


        public void Mutate()
        {
            //mutate always cause atleast one new mutation
            while (!IsDirty)
            {
                if (Tools.WillMutate(Settings.ActiveAddPolygonMutationRate))
                    AddPolygon();

                if (Tools.WillMutate(Settings.ActiveRemovePolygonMutationRate))
                    RemovePolygon();

                if (Tools.WillMutate(Settings.ActiveMovePolygonMutationRate))
                    MovePolygon();

                foreach (DnaPolygon polygon in Polygons)
                    polygon.Mutate(this);
            }
        }

        public void MovePolygon()
        {
            if (Polygons.Count < 1)
                return;

            int index = Tools.GetRandomNumber(0, Polygons.Count);
            DnaPolygon poly = Polygons[index];
            Polygons.RemoveAt(index);
            index = Tools.GetRandomNumber(0, Polygons.Count);
            Polygons.Insert(index, poly);
            SetDirty();
        }

        public void RemovePolygon()
        {
            if (Polygons.Count > Settings.ActivePolygonsMin)
            {
                int index = Tools.GetRandomNumber(0, Polygons.Count);
                Polygons.RemoveAt(index);
                SetDirty();
            }
        }

        public void AddPolygon()
        {
            if (Polygons.Count < Settings.ActivePolygonsMax)
            {
                if (Polygons.Count > 5)
                {
                    DnaPolygon master = Polygons[Tools.GetRandomNumber(0, Polygons.Count)];
                    var newPolygon = master.Clone();
                    Polygons.Insert(Polygons.IndexOf(master), newPolygon);
                }
                else
                {
                    var newPolygon = new DnaPolygon();
                    newPolygon.Init(this);

                    int index = Tools.GetRandomNumber(0, Polygons.Count);

                    Polygons.Insert(index, newPolygon);
                }
                SetDirty();
            }
        }
    }
}