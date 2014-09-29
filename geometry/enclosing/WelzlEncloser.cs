using System.Collections.Generic;

/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace mathlib.geometry.enclosing
{


	using MathInternalError = mathlib.exception.MathInternalError;
	using mathlib.geometry;

	/// <summary>
	/// Class implementing Emo Welzl algorithm to find the smallest enclosing ball in linear time.
	/// <p>
	/// The class implements the algorithm described in paper <a
	/// href="http://www.inf.ethz.ch/personal/emo/PublFiles/SmallEnclDisk_LNCS555_91.pdf">Smallest
	/// Enclosing Disks (Balls and Ellipsoids)</a> by Emo Welzl, Lecture Notes in Computer Science
	/// 555 (1991) 359-370. The pivoting improvement published in the paper <a
	/// href="http://www.inf.ethz.ch/personal/gaertner/texts/own_work/esa99_final.pdf">Fast and
	/// Robust Smallest Enclosing Balls</a>, by Bernd Gärtner and further modified in
	/// paper <a
	/// href=http://www.idt.mdh.se/kurser/ct3340/ht12/MINICONFERENCE/FinalPapers/ircse12_submission_30.pdf">
	/// Efficient Computation of Smallest Enclosing Balls in Three Dimensions</a> by Linus Källberg
	/// to avoid performing local copies of data have been included.
	/// </p> </summary>
	/// @param <S> Space type. </param>
	/// @param <P> Point type.
	/// @version $Id: WelzlEncloser.java 1591835 2014-05-02 09:04:01Z tn $
	/// @since 3.3 </param>
	public class WelzlEncloser<S, P> : Encloser<S, P> where S : mathlib.geometry.Space where P : mathlib.geometry.Point<S>
	{

		/// <summary>
		/// Tolerance below which points are consider to be identical. </summary>
		private readonly double tolerance;

		/// <summary>
		/// Generator for balls on support. </summary>
		private readonly SupportBallGenerator<S, P> generator;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="tolerance"> below which points are consider to be identical </param>
		/// <param name="generator"> generator for balls on support </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public WelzlEncloser(final double tolerance, final SupportBallGenerator<S, P> generator)
		public WelzlEncloser(double tolerance, SupportBallGenerator<S, P> generator)
		{
			this.tolerance = tolerance;
			this.generator = generator;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public EnclosingBall<S, P> enclose(final Iterable<P> points)
		public virtual EnclosingBall<S, P> enclose(IEnumerable<P> points)
		{

			if (points == null || !points.GetEnumerator().hasNext())
			{
				// return an empty ball
				return generator.ballOnSupport(new List<P>());
			}

			// Emo Welzl algorithm with Bernd Gärtner and Linus Källberg improvements
			return pivotingBall(points);

		}

		/// <summary>
		/// Compute enclosing ball using Gärtner's pivoting heuristic. </summary>
		/// <param name="points"> points to be enclosed </param>
		/// <returns> enclosing ball </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private EnclosingBall<S, P> pivotingBall(final Iterable<P> points)
		private EnclosingBall<S, P> pivotingBall(IEnumerable<P> points)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final P first = points.iterator().next();
			P first = points.GetEnumerator().next();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<P> extreme = new java.util.ArrayList<P>(first.getSpace().getDimension() + 1);
			IList<P> extreme = new List<P>(first.Space.Dimension + 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<P> support = new java.util.ArrayList<P>(first.getSpace().getDimension() + 1);
			IList<P> support = new List<P>(first.Space.Dimension + 1);

			// start with only first point selected as a candidate support
			extreme.Add(first);
			EnclosingBall<S, P> ball = moveToFrontBall(extreme, extreme.Count, support);

			while (true)
			{

				// select the point farthest to current ball
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final P farthest = selectFarthest(points, ball);
				P farthest = selectFarthest(points, ball);

				if (ball.contains(farthest, tolerance))
				{
					// we have found a ball containing all points
					return ball;
				}

				// recurse search, restricted to the small subset containing support and farthest point
				support.Clear();
				support.Add(farthest);
				EnclosingBall<S, P> savedBall = ball;
				ball = moveToFrontBall(extreme, extreme.Count, support);
				if (ball.Radius < savedBall.Radius)
				{
					// this should never happen
					throw new MathInternalError();
				}

				// it was an interesting point, move it to the front
				// according to Gärtner's heuristic
				extreme.Insert(0, farthest);

				// prune the least interesting points
				extreme.subList(ball.SupportSize, extreme.Count).clear();


			}
		}

		/// <summary>
		/// Compute enclosing ball using Welzl's move to front heuristic. </summary>
		/// <param name="extreme"> subset of extreme points </param>
		/// <param name="nbExtreme"> number of extreme points to consider </param>
		/// <param name="support"> points that must belong to the ball support </param>
		/// <returns> enclosing ball, for the extreme subset only </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private EnclosingBall<S, P> moveToFrontBall(final java.util.List<P> extreme, final int nbExtreme, final java.util.List<P> support)
		private EnclosingBall<S, P> moveToFrontBall(IList<P> extreme, int nbExtreme, IList<P> support)
		{

			// create a new ball on the prescribed support
			EnclosingBall<S, P> ball = generator.ballOnSupport(support);

			if (ball.SupportSize <= ball.Center.Space.Dimension)
			{

				for (int i = 0; i < nbExtreme; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final P pi = extreme.get(i);
					P pi = extreme[i];
					if (!ball.contains(pi, tolerance))
					{

						// we have found an outside point,
						// enlarge the ball by adding it to the support
						support.Add(pi);
						ball = moveToFrontBall(extreme, i, support);
						support.Remove(support.Count - 1);

						// it was an interesting point, move it to the front
						// according to Welzl's heuristic
						for (int j = i; j > 0; --j)
						{
							extreme[j] = extreme[j - 1];
						}
						extreme[0] = pi;

					}
				}

			}

			return ball;

		}

		/// <summary>
		/// Select the point farthest to the current ball. </summary>
		/// <param name="points"> points to be enclosed </param>
		/// <param name="ball"> current ball </param>
		/// <returns> farthest point </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public P selectFarthest(final Iterable<P> points, final EnclosingBall<S, P> ball)
		public virtual P selectFarthest(IEnumerable<P> points, EnclosingBall<S, P> ball)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final P center = ball.getCenter();
			P center = ball.Center;
			P farthest = null;
			double dMax = -1.0;

			foreach (P point in points)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d = point.distance(center);
				double d = point.distance(center);
				if (d > dMax)
				{
					farthest = point;
					dMax = d;
				}
			}

			return farthest;

		}

	}

}