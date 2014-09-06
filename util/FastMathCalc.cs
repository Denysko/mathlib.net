using System;

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
namespace mathlib.util
{

    using DimensionMismatchException = mathlib.exception.DimensionMismatchException;

	/// <summary>
	/// Class used to compute the classical functions tables.
	/// @version $Id: FastMathCalc.java 1538368 2013-11-03 13:57:37Z erans $
	/// @since 3.0
	/// </summary>
	internal class FastMathCalc
	{

		/// <summary>
		/// 0x40000000 - used to split a double into two parts, both with the low order bits cleared.
		/// Equivalent to 2^30.
		/// </summary>
		private const long HEX_40000000 = 0x40000000L; // 1073741824L

		/// <summary>
		/// Factorial table, for Taylor series expansions. 0!, 1!, 2!, ... 19! </summary>
		private static readonly double[] FACT = new double[] {+1.0d, +1.0d, +2.0d, +6.0d, +24.0d, +120.0d, +720.0d, +5040.0d, +40320.0d, +362880.0d, +3628800.0d, +39916800.0d, +479001600.0d, +6227020800.0d, +87178291200.0d, +1307674368000.0d, +20922789888000.0d, +355687428096000.0d, +6402373705728000.0d, +121645100408832000.0d};

		/// <summary>
		/// Coefficients for slowLog. </summary>
		private static readonly double[][] LN_SPLIT_COEF = new double[][] {new double[] {2.0, 0.0}, new double[] {0.6666666269302368, 3.9736429850260626E-8}, new double[] {0.3999999761581421, 2.3841857910019882E-8}, new double[] {0.2857142686843872, 1.7029898543501842E-8}, new double[] {0.2222222089767456, 1.3245471311735498E-8}, new double[] {0.1818181574344635, 2.4384203044354907E-8}, new double[] {0.1538461446762085, 9.140260083262505E-9}, new double[] {0.13333332538604736, 9.220590270857665E-9}, new double[] {0.11764700710773468, 1.2393345855018391E-8}, new double[] {0.10526403784751892, 8.251545029714408E-9}, new double[] {0.0952233225107193, 1.2675934823758863E-8}, new double[] {0.08713622391223907, 1.1430250008909141E-8}, new double[] {0.07842259109020233, 2.404307984052299E-9}, new double[] {0.08371849358081818, 1.176342548272881E-8}, new double[] {0.030589580535888672, 1.2958646899018938E-9}, new double[] {0.14982303977012634, 1.225743062930824E-8}};

		/// <summary>
		/// Table start declaration. </summary>
		private const string TABLE_START_DECL = "    {";

		/// <summary>
		/// Table end declaration. </summary>
		private const string TABLE_END_DECL = "    };";

		/// <summary>
		/// Private Constructor.
		/// </summary>
		private FastMathCalc()
		{
		}

		/// <summary>
		/// Build the sine and cosine tables. </summary>
		/// <param name="SINE_TABLE_A"> table of the most significant part of the sines </param>
		/// <param name="SINE_TABLE_B"> table of the least significant part of the sines </param>
		/// <param name="COSINE_TABLE_A"> table of the most significant part of the cosines </param>
		/// <param name="COSINE_TABLE_B"> table of the most significant part of the cosines </param>
		/// <param name="SINE_TABLE_LEN"> length of the tables </param>
		/// <param name="TANGENT_TABLE_A"> table of the most significant part of the tangents </param>
		/// <param name="TANGENT_TABLE_B"> table of the most significant part of the tangents </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unused") private static void buildSinCosTables(double[] SINE_TABLE_A, double[] SINE_TABLE_B, double[] COSINE_TABLE_A, double[] COSINE_TABLE_B, int SINE_TABLE_LEN, double[] TANGENT_TABLE_A, double[] TANGENT_TABLE_B)
		private static void buildSinCosTables(double[] SINE_TABLE_A, double[] SINE_TABLE_B, double[] COSINE_TABLE_A, double[] COSINE_TABLE_B, int SINE_TABLE_LEN, double[] TANGENT_TABLE_A, double[] TANGENT_TABLE_B)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double result[] = new double[2];
			double[] result = new double[2];

			/* Use taylor series for 0 <= x <= 6/8 */
			for (int i = 0; i < 7; i++)
			{
				double x = i / 8.0;

				slowSin(x, result);
				SINE_TABLE_A[i] = result[0];
				SINE_TABLE_B[i] = result[1];

				slowCos(x, result);
				COSINE_TABLE_A[i] = result[0];
				COSINE_TABLE_B[i] = result[1];
			}

			/* Use angle addition formula to complete table to 13/8, just beyond pi/2 */
			for (int i = 7; i < SINE_TABLE_LEN; i++)
			{
				double[] xs = new double[2];
				double[] ys = new double[2];
				double[] @as = new double[2];
				double[] bs = new double[2];
				double[] temps = new double[2];

				if ((i & 1) == 0)
				{
					// Even, use double angle
					xs[0] = SINE_TABLE_A[i / 2];
					xs[1] = SINE_TABLE_B[i / 2];
					ys[0] = COSINE_TABLE_A[i / 2];
					ys[1] = COSINE_TABLE_B[i / 2];

					/* compute sine */
					splitMult(xs, ys, result);
					SINE_TABLE_A[i] = result[0] * 2.0;
					SINE_TABLE_B[i] = result[1] * 2.0;

					/* Compute cosine */
					splitMult(ys, ys, @as);
					splitMult(xs, xs, temps);
					temps[0] = -temps[0];
					temps[1] = -temps[1];
					splitAdd(@as, temps, result);
					COSINE_TABLE_A[i] = result[0];
					COSINE_TABLE_B[i] = result[1];
				}
				else
				{
					xs[0] = SINE_TABLE_A[i / 2];
					xs[1] = SINE_TABLE_B[i / 2];
					ys[0] = COSINE_TABLE_A[i / 2];
					ys[1] = COSINE_TABLE_B[i / 2];
					@as[0] = SINE_TABLE_A[i / 2 + 1];
					@as[1] = SINE_TABLE_B[i / 2 + 1];
					bs[0] = COSINE_TABLE_A[i / 2 + 1];
					bs[1] = COSINE_TABLE_B[i / 2 + 1];

					/* compute sine */
					splitMult(xs, bs, temps);
					splitMult(ys, @as, result);
					splitAdd(result, temps, result);
					SINE_TABLE_A[i] = result[0];
					SINE_TABLE_B[i] = result[1];

					/* Compute cosine */
					splitMult(ys, bs, result);
					splitMult(xs, @as, temps);
					temps[0] = -temps[0];
					temps[1] = -temps[1];
					splitAdd(result, temps, result);
					COSINE_TABLE_A[i] = result[0];
					COSINE_TABLE_B[i] = result[1];
				}
			}

			/* Compute tangent = sine/cosine */
			for (int i = 0; i < SINE_TABLE_LEN; i++)
			{
				double[] xs = new double[2];
				double[] ys = new double[2];
				double[] @as = new double[2];

				@as[0] = COSINE_TABLE_A[i];
				@as[1] = COSINE_TABLE_B[i];

				splitReciprocal(@as, ys);

				xs[0] = SINE_TABLE_A[i];
				xs[1] = SINE_TABLE_B[i];

				splitMult(xs, ys, @as);

				TANGENT_TABLE_A[i] = @as[0];
				TANGENT_TABLE_B[i] = @as[1];
			}

		}

		/// <summary>
		///  For x between 0 and pi/4 compute cosine using Talor series
		///  cos(x) = 1 - x^2/2! + x^4/4! ... </summary>
		/// <param name="x"> number from which cosine is requested </param>
		/// <param name="result"> placeholder where to put the result in extended precision
		/// (may be null) </param>
		/// <returns> cos(x) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: static double slowCos(final double x, final double result[])
		internal static double slowCos(double x, double[] result)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xs[] = new double[2];
			double[] xs = new double[2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ys[] = new double[2];
			double[] ys = new double[2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double facts[] = new double[2];
			double[] facts = new double[2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double as[] = new double[2];
			double[] @as = new double[2];
			Split(x, xs);
			ys[0] = ys[1] = 0.0;

			for (int i = FACT.Length - 1; i >= 0; i--)
			{
				splitMult(xs, ys, @as);
				ys[0] = @as[0];
				ys[1] = @as[1];

				if ((i & 1) != 0) // skip odd entries
				{
					continue;
				}

				Split(FACT[i], @as);
				splitReciprocal(@as, facts);

				if ((i & 2) != 0) // alternate terms are negative
				{
					facts[0] = -facts[0];
					facts[1] = -facts[1];
				}

				splitAdd(ys, facts, @as);
				ys[0] = @as[0];
				ys[1] = @as[1];
			}

			if (result != null)
			{
				result[0] = ys[0];
				result[1] = ys[1];
			}

			return ys[0] + ys[1];
		}

		/// <summary>
		/// For x between 0 and pi/4 compute sine using Taylor expansion:
		/// sin(x) = x - x^3/3! + x^5/5! - x^7/7! ... </summary>
		/// <param name="x"> number from which sine is requested </param>
		/// <param name="result"> placeholder where to put the result in extended precision
		/// (may be null) </param>
		/// <returns> sin(x) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: static double slowSin(final double x, final double result[])
		internal static double slowSin(double x, double[] result)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xs[] = new double[2];
			double[] xs = new double[2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ys[] = new double[2];
			double[] ys = new double[2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double facts[] = new double[2];
			double[] facts = new double[2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double as[] = new double[2];
			double[] @as = new double[2];
			Split(x, xs);
			ys[0] = ys[1] = 0.0;

			for (int i = FACT.Length - 1; i >= 0; i--)
			{
				splitMult(xs, ys, @as);
				ys[0] = @as[0];
				ys[1] = @as[1];

				if ((i & 1) == 0) // Ignore even numbers
				{
					continue;
				}

				Split(FACT[i], @as);
				splitReciprocal(@as, facts);

				if ((i & 2) != 0) // alternate terms are negative
				{
					facts[0] = -facts[0];
					facts[1] = -facts[1];
				}

				splitAdd(ys, facts, @as);
				ys[0] = @as[0];
				ys[1] = @as[1];
			}

			if (result != null)
			{
				result[0] = ys[0];
				result[1] = ys[1];
			}

			return ys[0] + ys[1];
		}


		/// <summary>
		///  For x between 0 and 1, returns exp(x), uses extended precision </summary>
		///  <param name="x"> argument of exponential </param>
		///  <param name="result"> placeholder where to place exp(x) split in two terms
		///  for extra precision (i.e. exp(x) = result[0] + result[1] </param>
		///  <returns> exp(x) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: static double slowexp(final double x, final double result[])
		internal static double slowexp(double x, double[] result)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xs[] = new double[2];
			double[] xs = new double[2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ys[] = new double[2];
			double[] ys = new double[2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double facts[] = new double[2];
			double[] facts = new double[2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double as[] = new double[2];
			double[] @as = new double[2];
			Split(x, xs);
			ys[0] = ys[1] = 0.0;

			for (int i = FACT.Length - 1; i >= 0; i--)
			{
				splitMult(xs, ys, @as);
				ys[0] = @as[0];
				ys[1] = @as[1];

				Split(FACT[i], @as);
				splitReciprocal(@as, facts);

				splitAdd(ys, facts, @as);
				ys[0] = @as[0];
				ys[1] = @as[1];
			}

			if (result != null)
			{
				result[0] = ys[0];
				result[1] = ys[1];
			}

			return ys[0] + ys[1];
		}

		/// <summary>
		/// Compute split[0], split[1] such that their sum is equal to d,
		/// and split[0] has its 30 least significant bits as zero. </summary>
		/// <param name="d"> number to split </param>
		/// <param name="split"> placeholder where to place the result </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void split(final double d, final double split[])
		private static void Split(double d, double[] split)
		{
			if (d < 8e298 && d > -8e298)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a = d * HEX_40000000;
				double a = d * HEX_40000000;
				split[0] = (d + a) - a;
				split[1] = d - split[0];
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a = d * 9.31322574615478515625E-10;
				double a = d * 9.31322574615478515625E-10;
				split[0] = (d + a - d) * HEX_40000000;
				split[1] = d - split[0];
			}
		}

		/// <summary>
		/// Recompute a split. </summary>
		/// <param name="a"> input/out array containing the split, changed
		/// on output </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void resplit(final double a[])
		private static void resplit(double[] a)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double c = a[0] + a[1];
			double c = a[0] + a[1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d = -(c - a[0] - a[1]);
			double d = -(c - a[0] - a[1]);

			if (c < 8e298 && c > -8e298) // MAGIC NUMBER
			{
				double z = c * HEX_40000000;
				a[0] = (c + z) - z;
				a[1] = c - a[0] + d;
			}
			else
			{
				double z = c * 9.31322574615478515625E-10;
				a[0] = (c + z - c) * HEX_40000000;
				a[1] = c - a[0] + d;
			}
		}

		/// <summary>
		/// Multiply two numbers in split form. </summary>
		/// <param name="a"> first term of multiplication </param>
		/// <param name="b"> second term of multiplication </param>
		/// <param name="ans"> placeholder where to put the result </param>
		private static void splitMult(double[] a, double[] b, double[] ans)
		{
			ans[0] = a[0] * b[0];
			ans[1] = a[0] * b[1] + a[1] * b[0] + a[1] * b[1];

			/* Resplit */
			resplit(ans);
		}

		/// <summary>
		/// Add two numbers in split form. </summary>
		/// <param name="a"> first term of addition </param>
		/// <param name="b"> second term of addition </param>
		/// <param name="ans"> placeholder where to put the result </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void splitAdd(final double a[] , final double b[], final double ans[])
		private static void splitAdd(double[] a, double[] b, double[] ans)
		{
			ans[0] = a[0] + b[0];
			ans[1] = a[1] + b[1];

			resplit(ans);
		}

		/// <summary>
		/// Compute the reciprocal of in.  Use the following algorithm.
		///  in = c + d.
		///  want to find x + y such that x+y = 1/(c+d) and x is much
		///  larger than y and x has several zero bits on the right.
		/// 
		///  Set b = 1/(2^22),  a = 1 - b.  Thus (a+b) = 1.
		///  Use following identity to compute (a+b)/(c+d)
		/// 
		///  (a+b)/(c+d)  =   a/c   +    (bc - ad) / (c^2 + cd)
		///  set x = a/c  and y = (bc - ad) / (c^2 + cd)
		///  This will be close to the right answer, but there will be
		///  some rounding in the calculation of X.  So by carefully
		///  computing 1 - (c+d)(x+y) we can compute an error and
		///  add that back in.   This is done carefully so that terms
		///  of similar size are subtracted first. </summary>
		///  <param name="in"> initial number, in split form </param>
		///  <param name="result"> placeholder where to put the result </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: static void splitReciprocal(final double in[] , final double result[])
		internal static void splitReciprocal(double[] @in, double[] result)
		{
			const double b = 1.0 / 4194304.0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a = 1.0 - b;
			double a = 1.0 - b;

			if (@in[0] == 0.0)
			{
				@in[0] = @in[1];
				@in[1] = 0.0;
			}

			result[0] = a / @in[0];
			result[1] = (b * @in[0] - a * @in[1]) / (@in[0] * @in[0] + @in[0] * @in[1]);

			if (result[1] != result[1]) // can happen if result[1] is NAN
			{
				result[1] = 0.0;
			}

			/* Resplit */
			resplit(result);

			for (int i = 0; i < 2; i++)
			{
				/* this may be overkill, probably once is enough */
				double err = 1.0 - result[0] * @in[0] - result[0] * @in[1] - result[1] * @in[0] - result[1] * @in[1];
				/*err = 1.0 - err; */
				err *= result[0] + result[1];
				/*printf("err = %16e\n", err); */
				result[1] += err;
			}
		}

		/// <summary>
		/// Compute (a[0] + a[1]) * (b[0] + b[1]) in extended precision. </summary>
		/// <param name="a"> first term of the multiplication </param>
		/// <param name="b"> second term of the multiplication </param>
		/// <param name="result"> placeholder where to put the result </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void quadMult(final double a[] , final double b[], final double result[])
		private static void quadMult(double[] a, double[] b, double[] result)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xs[] = new double[2];
			double[] xs = new double[2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ys[] = new double[2];
			double[] ys = new double[2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double zs[] = new double[2];
			double[] zs = new double[2];

			/* a[0] * b[0] */
			Split(a[0], xs);
			Split(b[0], ys);
			splitMult(xs, ys, zs);

			result[0] = zs[0];
			result[1] = zs[1];

			/* a[0] * b[1] */
			Split(b[1], ys);
			splitMult(xs, ys, zs);

			double tmp = result[0] + zs[0];
			result[1] -= tmp - result[0] - zs[0];
			result[0] = tmp;
			tmp = result[0] + zs[1];
			result[1] -= tmp - result[0] - zs[1];
			result[0] = tmp;

			/* a[1] * b[0] */
			Split(a[1], xs);
			Split(b[0], ys);
			splitMult(xs, ys, zs);

			tmp = result[0] + zs[0];
			result[1] -= tmp - result[0] - zs[0];
			result[0] = tmp;
			tmp = result[0] + zs[1];
			result[1] -= tmp - result[0] - zs[1];
			result[0] = tmp;

			/* a[1] * b[0] */
			Split(a[1], xs);
			Split(b[1], ys);
			splitMult(xs, ys, zs);

			tmp = result[0] + zs[0];
			result[1] -= tmp - result[0] - zs[0];
			result[0] = tmp;
			tmp = result[0] + zs[1];
			result[1] -= tmp - result[0] - zs[1];
			result[0] = tmp;
		}

		/// <summary>
		/// Compute exp(p) for a integer p in extended precision. </summary>
		/// <param name="p"> integer whose exponential is requested </param>
		/// <param name="result"> placeholder where to put the result in extended precision </param>
		/// <returns> exp(p) in standard precision (equal to result[0] + result[1]) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: static double expint(int p, final double result[])
		internal static double expint(int p, double[] result)
		{
			//double x = M_E;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xs[] = new double[2];
			double[] xs = new double[2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double as[] = new double[2];
			double[] @as = new double[2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ys[] = new double[2];
			double[] ys = new double[2];
			//split(x, xs);
			//xs[1] = (double)(2.7182818284590452353602874713526625L - xs[0]);
			//xs[0] = 2.71827697753906250000;
			//xs[1] = 4.85091998273542816811e-06;
			//xs[0] = Double.longBitsToDouble(0x4005bf0800000000L);
			//xs[1] = Double.longBitsToDouble(0x3ed458a2bb4a9b00L);

			/* E */
			xs[0] = 2.718281828459045;
			xs[1] = 1.4456468917292502E-16;

			Split(1.0, ys);

			while (p > 0)
			{
				if ((p & 1) != 0)
				{
					quadMult(ys, xs, @as);
					ys[0] = @as[0];
					ys[1] = @as[1];
				}

				quadMult(xs, xs, @as);
				xs[0] = @as[0];
				xs[1] = @as[1];

				p >>= 1;
			}

			if (result != null)
			{
				result[0] = ys[0];
				result[1] = ys[1];

				resplit(result);
			}

			return ys[0] + ys[1];
		}
		/// <summary>
		/// xi in the range of [1, 2].
		///                                3        5        7
		///      x+1           /          x        x        x          \
		///  ln ----- =   2 *  |  x  +   ----  +  ----  +  ---- + ...  |
		///      1-x           \          3        5        7          /
		/// 
		/// So, compute a Remez approximation of the following function
		/// 
		///  ln ((sqrt(x)+1)/(1-sqrt(x)))  /  x
		/// 
		/// This will be an even function with only positive coefficents.
		/// x is in the range [0 - 1/3].
		/// 
		/// Transform xi for input to the above function by setting
		/// x = (xi-1)/(xi+1).   Input to the polynomial is x^2, then
		/// the result is multiplied by x. </summary>
		/// <param name="xi"> number from which log is requested </param>
		/// <returns> log(xi) </returns>
		internal static double[] slowLog(double xi)
		{
			double[] x = new double[2];
			double[] x2 = new double[2];
			double[] y = new double[2];
			double[] a = new double[2];

			Split(xi, x);

			/* Set X = (x-1)/(x+1) */
			x[0] += 1.0;
			resplit(x);
			splitReciprocal(x, a);
			x[0] -= 2.0;
			resplit(x);
			splitMult(x, a, y);
			x[0] = y[0];
			x[1] = y[1];

			/* Square X -> X2*/
			splitMult(x, x, x2);


			//x[0] -= 1.0;
			//resplit(x);

			y[0] = LN_SPLIT_COEF[LN_SPLIT_COEF.Length - 1][0];
			y[1] = LN_SPLIT_COEF[LN_SPLIT_COEF.Length - 1][1];

			for (int i = LN_SPLIT_COEF.Length - 2; i >= 0; i--)
			{
				splitMult(y, x2, a);
				y[0] = a[0];
				y[1] = a[1];
				splitAdd(y, LN_SPLIT_COEF[i], a);
				y[0] = a[0];
				y[1] = a[1];
			}

			splitMult(y, x, a);
			y[0] = a[0];
			y[1] = a[1];

			return y;
		}


		/// <summary>
		/// Print an array. </summary>
		/// <param name="out"> text output stream where output should be printed </param>
		/// <param name="name"> array name </param>
		/// <param name="expectedLen"> expected length of the array </param>
		/// <param name="array2d"> array data </param>
		internal static void printarray(PrintStream @out, string name, int expectedLen, double[][] array2d)
		{
			@out.println(name);
			checkLen(expectedLen, array2d.Length);
			@out.println(TABLE_START_DECL + " ");
			int i = 0;
			foreach (double[] array in array2d) // "double array[]" causes PMD parsing error
			{
				@out.print("        {");
				foreach (double d in array) // assume inner array has very few entries
				{
					@out.printf("%-25.25s", format(d)); // multiple entries per line
				}
				@out.println("}, // " + i++);
			}
			@out.println(TABLE_END_DECL);
		}

		/// <summary>
		/// Print an array. </summary>
		/// <param name="out"> text output stream where output should be printed </param>
		/// <param name="name"> array name </param>
		/// <param name="expectedLen"> expected length of the array </param>
		/// <param name="array"> array data </param>
		internal static void printarray(PrintStream @out, string name, int expectedLen, double[] array)
		{
			@out.println(name + "=");
			checkLen(expectedLen, array.Length);
			@out.println(TABLE_START_DECL);
			foreach (double d in array)
			{
				@out.printf("        %s%n", format(d)); // one entry per line
			}
			@out.println(TABLE_END_DECL);
		}

		/// <summary>
		/// Format a double. </summary>
		/// <param name="d"> double number to format </param>
		/// <returns> formatted number </returns>
		internal static string format(double d)
		{
			if (d != d)
			{
				return "Double.NaN,";
			}
			else
			{
				return ((d >= 0) ? "+" : "") + Convert.ToString(d) + "d,";
			}
		}

		/// <summary>
		/// Check two lengths are equal. </summary>
		/// <param name="expectedLen"> expected length </param>
		/// <param name="actual"> actual length </param>
		/// <exception cref="DimensionMismatchException"> if the two lengths are not equal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void checkLen(int expectedLen, int actual) throws org.apache.commons.math3.exception.DimensionMismatchException
		private static void checkLen(int expectedLen, int actual)
		{
			if (expectedLen != actual)
			{
				throw new DimensionMismatchException(actual, expectedLen);
			}
		}

	}

}