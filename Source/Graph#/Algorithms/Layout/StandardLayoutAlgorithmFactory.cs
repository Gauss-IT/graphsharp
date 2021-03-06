﻿using System;
using System.Collections.Generic;
using System.Linq;
using GraphSharp.Algorithms.Layout.Simple.Grid;
using QuickGraph;
using GraphSharp.Algorithms.Layout.Simple.Tree;
using GraphSharp.Algorithms.Layout.Simple.Circular;
using GraphSharp.Algorithms.Layout.Simple.FDP;
using GraphSharp.Algorithms.Layout.Simple.Hierarchical;
using GraphSharp.Algorithms.Layout.Compound;
using GraphSharp.Algorithms.Layout.Compound.FDP;
using System.Windows;

namespace GraphSharp.Algorithms.Layout
{
    public class StandardLayoutAlgorithmFactory<TVertex, TEdge, TGraph> : ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        public IEnumerable<string> AlgorithmTypes
        {
            get { return new[] { "Circular", "Tree", "FR", "BoundedFR", "KK", "ISOM", "LinLog", "EfficientSugiyama", /*"Sugiyama",*/ "CompoundFDP", "StressMajorization", "Grid" }; }
        }

        public ILayoutAlgorithm<TVertex, TEdge, TGraph> CreateAlgorithm(string newAlgorithmType, ILayoutContext<TVertex, TEdge, TGraph> context, ILayoutParameters parameters)
        {
            if (context == null || context.Graph == null)
                return null;

            if (context.Mode == LayoutMode.Simple)
            {
                switch (newAlgorithmType)
                {
                    case "Tree":
                        return new SimpleTreeLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph, context.Positions,
                                                                                     context.Sizes,
                                                                                     parameters as SimpleTreeLayoutParameters);
                    case "Circular":
                        return new CircularLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph, context.Positions,
                                                                                   context.Sizes,
                                                                                   parameters as CircularLayoutParameters);
                    case "FR":
                        return new FRLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph, context.Positions,
                                                                             parameters as FRLayoutParametersBase);
                    case "BoundedFR":
                        return new FRLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph, context.Positions,
                                                                             parameters as BoundedFRLayoutParameters);
                    case "KK":
                        return new KKLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph, context.Positions,
                                                                             parameters as KKLayoutParameters);
                    case "ISOM":
                        return new ISOMLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph, context.Positions,
                                                                               parameters as ISOMLayoutParameters);
                    case "LinLog":
                        return new LinLogLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph, context.Positions,
                                                                                 parameters as LinLogLayoutParameters);
                    case "EfficientSugiyama":
                        return new EfficientSugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph, 
                                                                                            parameters as EfficientSugiyamaLayoutParameters,
                                                                                            context.Positions,
                                                                                            context.Sizes);

                    case "Sugiyama":
                        return new SugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph, context.Sizes,
                                                                                   context.Positions,
                                                                                   parameters as
                                                                                   SugiyamaLayoutParameters,
                                                                                   e => (e is ITypedEdge<TVertex>
                                                                                        ? (e as ITypedEdge<TVertex>).Type
                                                                                        : EdgeTypes.Hierarchical));
                    case "CompoundFDP":
                        return new CompoundFDPLayoutAlgorithm<TVertex, TEdge, TGraph>(
                            context.Graph,
                            context.Sizes,
                            new Dictionary<TVertex, Thickness>(),
                            new Dictionary<TVertex, CompoundVertexInnerLayoutType>(),
                            context.Positions,
                            parameters as CompoundFDPLayoutParameters);

                    case "StressMajorization":
                        return new StressMajorizationLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph, context.Positions, parameters as StressMajorizationLayoutParameters);

                    case "Grid":
                        return new GridLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph, context.Positions, context.Sizes, parameters as GridLayoutParameters);

                    default:
                        return null;
                }
            }

            if (context.Mode == LayoutMode.Compound)
            {
                var compoundContext = (ICompoundLayoutContext<TVertex, TEdge, TGraph>) context;
                switch (newAlgorithmType)
                {
                    case "CompoundFDP":
                        return new CompoundFDPLayoutAlgorithm<TVertex, TEdge, TGraph>(
                            compoundContext.Graph,
                            compoundContext.Sizes,
                            compoundContext.VertexBorders,
                            compoundContext.LayoutTypes,
                            compoundContext.Positions,
                            parameters as CompoundFDPLayoutParameters);
                    default:
                        return null;
                }
            }
            return null;
        }

        public ILayoutParameters CreateParameters(string algorithmType, ILayoutParameters oldParameters)
        {
            switch (algorithmType)
            {
                case "Tree":
                    return oldParameters.CreateNewParameter<SimpleTreeLayoutParameters>();
                case "Circular":
                    return oldParameters.CreateNewParameter<CircularLayoutParameters>();
                case "FR":
                    return oldParameters.CreateNewParameter<FreeFRLayoutParameters>();
                case "BoundedFR":
                    return oldParameters.CreateNewParameter<BoundedFRLayoutParameters>();
                case "KK":
                    return oldParameters.CreateNewParameter<KKLayoutParameters>();
                case "ISOM":
                    return oldParameters.CreateNewParameter<ISOMLayoutParameters>();
                case "LinLog":
                    return oldParameters.CreateNewParameter<LinLogLayoutParameters>();
                case "EfficientSugiyama":
                    return oldParameters.CreateNewParameter<EfficientSugiyamaLayoutParameters>();
                case "Sugiyama":
                    return oldParameters.CreateNewParameter<SugiyamaLayoutParameters>();
                case "CompoundFDP":
                    return oldParameters.CreateNewParameter<CompoundFDPLayoutParameters>();
                case "StressMajorization":
                    return oldParameters.CreateNewParameter<StressMajorizationLayoutParameters>();
                case "Grid":
                    return oldParameters.CreateNewParameter<GridLayoutParameters>();
                default:
                    return null;
            }
        }

        

        public bool IsValidAlgorithm(string algorithmType)
        {
            return AlgorithmTypes.Contains(algorithmType);
        }

        public string GetAlgorithmType(ILayoutAlgorithm<TVertex, TEdge, TGraph> algorithm)
        {
            if (algorithm == null)
                return string.Empty;

            int index = algorithm.GetType().Name.IndexOf("LayoutAlgorithm", StringComparison.Ordinal);
            if (index == -1)
                return string.Empty;

            string algoType = algorithm.GetType().Name;
            return algoType.Substring(0, algoType.Length - index);
        }

        public bool NeedEdgeRouting(string algorithmType)
        {
            switch (algorithmType)
            {
                case "Tree":
                case "Grid":
                    return true;
            }

            return false;
        }

        public bool NeedOverlapRemoval(string algorithmType)
        {
            switch (algorithmType)
            {
                case "FR":
                case "BoundedFR":
                case "KK":
                case "ISOM":
                case "LinLog":
                case "StressMajorization":
                    return true;
            }

            return false;
        }
    }
}

