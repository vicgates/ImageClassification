﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternRecognition
{
    public interface IClassyfiAlgorithm
    {
        int Classify(List<PatternClass> teachingVectors, List<double> classyfiedObject);
    }
}
