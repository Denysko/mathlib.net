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
namespace mathlib.exception.util
{


	/// <summary>
	/// Enumeration for localized messages formats used in exceptions messages.
	/// <p>
	/// The constants in this enumeration represent the available
	/// formats as localized strings. These formats are intended to be
	/// localized using simple properties files, using the constant
	/// name as the key and the property value as the message format.
	/// The source English format is provided in the constants themselves
	/// to serve both as a reminder for developers to understand the parameters
	/// needed by each format, as a basis for translators to create
	/// localized properties files, and as a default format if some
	/// translation is missing.
	/// </p>
	/// @since 2.2
	/// @version $Id: LocalizedFormats.java 1568752 2014-02-16 12:19:51Z tn $
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
//ORIGINAL LINE: public enum LocalizedFormats implements Localizable
	public enum LocalizedFormats
	{

		// CHECKSTYLE: stop MultipleVariableDeclarations
		// CHECKSTYLE: stop JavadocVariable

//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ARGUMENT_OUTSIDE_DOMAIN("Argument {0} outside domain [{1} ; {2}]"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ARRAY_SIZE_EXCEEDS_MAX_VARIABLES("array size cannot be greater than {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ARRAY_SIZES_SHOULD_HAVE_DIFFERENCE_1("array sizes should have difference 1 ({0} != {1} + 1)"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ARRAY_SUMS_TO_ZERO("array sums to zero"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ASSYMETRIC_EIGEN_NOT_SUPPORTED("eigen decomposition of assymetric matrices not supported yet"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		AT_LEAST_ONE_COLUMN("matrix must have at least one column"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		AT_LEAST_ONE_ROW("matrix must have at least one row"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		BANDWIDTH("bandwidth ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		BINOMIAL_INVALID_PARAMETERS_ORDER("must have n >= k for binomial coefficient (n, k), got k = {0}, n = {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		BINOMIAL_NEGATIVE_PARAMETER("must have n >= 0 for binomial coefficient (n, k), got n = {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CANNOT_CLEAR_STATISTIC_CONSTRUCTED_FROM_EXTERNAL_MOMENTS("statistics constructed from external moments cannot be cleared"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CANNOT_COMPUTE_0TH_ROOT_OF_UNITY("cannot compute 0-th root of unity, indefinite result"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CANNOT_COMPUTE_BETA_DENSITY_AT_0_FOR_SOME_ALPHA("cannot compute beta density at 0 when alpha = {0,number}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CANNOT_COMPUTE_BETA_DENSITY_AT_1_FOR_SOME_BETA("cannot compute beta density at 1 when beta = %.3g"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CANNOT_COMPUTE_NTH_ROOT_FOR_NEGATIVE_N("cannot compute nth root for null or negative n: {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CANNOT_DISCARD_NEGATIVE_NUMBER_OF_ELEMENTS("cannot discard a negative number of elements ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CANNOT_FORMAT_INSTANCE_AS_3D_VECTOR("cannot format a {0} instance as a 3D vector"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CANNOT_FORMAT_INSTANCE_AS_COMPLEX("cannot format a {0} instance as a complex number"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CANNOT_FORMAT_INSTANCE_AS_REAL_VECTOR("cannot format a {0} instance as a real vector"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CANNOT_FORMAT_OBJECT_TO_FRACTION("cannot format given object as a fraction number"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CANNOT_INCREMENT_STATISTIC_CONSTRUCTED_FROM_EXTERNAL_MOMENTS("statistics constructed from external moments cannot be incremented"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CANNOT_NORMALIZE_A_ZERO_NORM_VECTOR("cannot normalize a zero norm vector"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CANNOT_RETRIEVE_AT_NEGATIVE_INDEX("elements cannot be retrieved from a negative array index {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CANNOT_SET_AT_NEGATIVE_INDEX("cannot set an element at a negative index {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CANNOT_SUBSTITUTE_ELEMENT_FROM_EMPTY_ARRAY("cannot substitute an element from an empty array"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CANNOT_TRANSFORM_TO_DOUBLE("Conversion Exception in Transformation: {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CARDAN_ANGLES_SINGULARITY("Cardan angles singularity"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CLASS_DOESNT_IMPLEMENT_COMPARABLE("class ({0}) does not implement Comparable"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CLOSEST_ORTHOGONAL_MATRIX_HAS_NEGATIVE_DETERMINANT("the closest orthogonal matrix has a negative determinant {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		COLUMN_INDEX_OUT_OF_RANGE("column index {0} out of allowed range [{1}, {2}]"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		COLUMN_INDEX("column index ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CONSTRAINT("constraint"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CONTINUED_FRACTION_INFINITY_DIVERGENCE("Continued fraction convergents diverged to +/- infinity for value {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CONTINUED_FRACTION_NAN_DIVERGENCE("Continued fraction diverged to NaN for value {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CONTRACTION_CRITERIA_SMALLER_THAN_EXPANSION_FACTOR("contraction criteria ({0}) smaller than the expansion factor ({1}).  This would lead to a never ending loop of expansion and contraction as a newly expanded internal storage array would immediately satisfy the criteria for contraction."),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CONTRACTION_CRITERIA_SMALLER_THAN_ONE("contraction criteria smaller than one ({0}).  This would lead to a never ending loop of expansion and contraction as an internal storage array length equal to the number of elements would satisfy the contraction criteria."),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CONVERGENCE_FAILED("convergence failed"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CROSSING_BOUNDARY_LOOPS("some outline boundary loops cross each other"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CROSSOVER_RATE("crossover rate ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CUMULATIVE_PROBABILITY_RETURNED_NAN("Cumulative probability function returned NaN for argument {0} p = {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		DIFFERENT_ROWS_LENGTHS("some rows have length {0} while others have length {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		DIFFERENT_ORIG_AND_PERMUTED_DATA("original and permuted data must contain the same elements"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		DIGEST_NOT_INITIALIZED("digest not initialized"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		DIMENSIONS_MISMATCH_2x2("got {0}x{1} but expected {2}x{3}"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		DIMENSIONS_MISMATCH_SIMPLE("{0} != {1}"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		DIMENSIONS_MISMATCH("dimensions mismatch"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		DISCRETE_CUMULATIVE_PROBABILITY_RETURNED_NAN("Discrete cumulative probability function returned NaN for argument {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		DISTRIBUTION_NOT_LOADED("distribution not loaded"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		DUPLICATED_ABSCISSA_DIVISION_BY_ZERO("duplicated abscissa {0} causes division by zero"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ELITISM_RATE("elitism rate ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		EMPTY_CLUSTER_IN_K_MEANS("empty cluster in k-means"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		EMPTY_INTERPOLATION_SAMPLE("sample for interpolation is empty"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		EMPTY_POLYNOMIALS_COEFFICIENTS_ARRAY("empty polynomials coefficients array"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		EMPTY_SELECTED_COLUMN_INDEX_ARRAY("empty selected column index array"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		EMPTY_SELECTED_ROW_INDEX_ARRAY("empty selected row index array"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		EMPTY_STRING_FOR_IMAGINARY_CHARACTER("empty string for imaginary character"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ENDPOINTS_NOT_AN_INTERVAL("endpoints do not specify an interval: [{0}, {1}]"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		EQUAL_VERTICES_IN_SIMPLEX("equal vertices {0} and {1} in simplex configuration"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		EULER_ANGLES_SINGULARITY("Euler angles singularity"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		EVALUATION("evaluation"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		EXPANSION_FACTOR_SMALLER_THAN_ONE("expansion factor smaller than one ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		FACTORIAL_NEGATIVE_PARAMETER("must have n >= 0 for n!, got n = {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		FAILED_BRACKETING("number of iterations={4}, maximum iterations={5}, initial={6}, lower bound={7}, upper bound={8}, final a value={0}, final b value={1}, f(a)={2}, f(b)={3}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		FAILED_FRACTION_CONVERSION("Unable to convert {0} to fraction after {1} iterations"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		FIRST_COLUMNS_NOT_INITIALIZED_YET("first {0} columns are not initialized yet"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		FIRST_ELEMENT_NOT_ZERO("first element is not 0: {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		FIRST_ROWS_NOT_INITIALIZED_YET("first {0} rows are not initialized yet"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		FRACTION_CONVERSION_OVERFLOW("Overflow trying to convert {0} to fraction ({1}/{2})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		FUNCTION_NOT_DIFFERENTIABLE("function is not differentiable"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		FUNCTION_NOT_POLYNOMIAL("function is not polynomial"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		GCD_OVERFLOW_32_BITS("overflow: gcd({0}, {1}) is 2^31"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		GCD_OVERFLOW_64_BITS("overflow: gcd({0}, {1}) is 2^63"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		HOLE_BETWEEN_MODELS_TIME_RANGES("{0} wide hole between models time ranges"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ILL_CONDITIONED_OPERATOR("condition number {1} is too high "),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INCONSISTENT_STATE_AT_2_PI_WRAPPING("inconsistent state at 2\u03c0 wrapping"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INDEX_LARGER_THAN_MAX("the index specified: {0} is larger than the current maximal index {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INDEX_NOT_POSITIVE("index ({0}) is not positive"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INDEX_OUT_OF_RANGE("index {0} out of allowed range [{1}, {2}]"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INDEX("index ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_FINITE_NUMBER("{0} is not a finite number"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INFINITE_BOUND("interval bounds must be finite"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ARRAY_ELEMENT("value {0} at index {1}"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INFINITE_ARRAY_ELEMENT("Array contains an infinite element, {0} at index {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INFINITE_VALUE_CONVERSION("cannot convert infinite value"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INITIAL_CAPACITY_NOT_POSITIVE("initial capacity ({0}) is not positive"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INITIAL_COLUMN_AFTER_FINAL_COLUMN("initial column {1} after final column {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INITIAL_ROW_AFTER_FINAL_ROW("initial row {1} after final row {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		[Obsolete]
		INPUT_DATA_FROM_UNSUPPORTED_DATASOURCE("input data comes from unsupported datasource: {0}, supported sources: {1}, {2}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INSTANCES_NOT_COMPARABLE_TO_EXISTING_VALUES("instance of class {0} not comparable to existing values"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INSUFFICIENT_DATA("insufficient data"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INSUFFICIENT_DATA_FOR_T_STATISTIC("insufficient data for t statistic, needs at least 2, got {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INSUFFICIENT_DIMENSION("insufficient dimension {0}, must be at least {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		DIMENSION("dimension ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INSUFFICIENT_OBSERVED_POINTS_IN_SAMPLE("sample contains {0} observed points, at least {1} are required"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INSUFFICIENT_ROWS_AND_COLUMNS("insufficient data: only {0} rows and {1} columns."),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INTEGRATION_METHOD_NEEDS_AT_LEAST_TWO_PREVIOUS_POINTS("multistep method needs at least {0} previous steps, got {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INTERNAL_ERROR("internal error, please fill a bug report at {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INVALID_BINARY_DIGIT("invalid binary digit: {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INVALID_BINARY_CHROMOSOME("binary mutation works on BinaryChromosome only"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INVALID_BRACKETING_PARAMETERS("invalid bracketing parameters:  lower bound={0},  initial={1}, upper bound={2}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INVALID_FIXED_LENGTH_CHROMOSOME("one-point crossover only works with fixed-length chromosomes"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INVALID_INTERVAL_INITIAL_VALUE_PARAMETERS("invalid interval, initial value parameters:  lower={0}, initial={1}, upper={2}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INVALID_ITERATIONS_LIMITS("invalid iteration limits: min={0}, max={1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INVALID_MAX_ITERATIONS("bad value for maximum iterations number: {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_ENOUGH_DATA_REGRESSION("the number of observations is not sufficient to conduct regression"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INVALID_REGRESSION_ARRAY("input data array length = {0} does not match the number of observations = {1} and the number of regressors = {2}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INVALID_REGRESSION_OBSERVATION("length of regressor array = {0} does not match the number of variables = {1} in the model"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INVALID_ROUNDING_METHOD("invalid rounding method {0}, valid methods: {1} ({2}), {3} ({4}), {5} ({6}), {7} ({8}), {9} ({10}), {11} ({12}), {13} ({14}), {15} ({16})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ITERATOR_EXHAUSTED("iterator exhausted"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ITERATIONS("iterations"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		LCM_OVERFLOW_32_BITS("overflow: lcm({0}, {1}) is 2^31"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		LCM_OVERFLOW_64_BITS("overflow: lcm({0}, {1}) is 2^63"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		LIST_OF_CHROMOSOMES_BIGGER_THAN_POPULATION_SIZE("list of chromosomes bigger than maxPopulationSize"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		LOESS_EXPECTS_AT_LEAST_ONE_POINT("Loess expects at least 1 point"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		LOWER_BOUND_NOT_BELOW_UPPER_BOUND("lower bound ({0}) must be strictly less than upper bound ({1})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		LOWER_ENDPOINT_ABOVE_UPPER_ENDPOINT("lower endpoint ({0}) must be less than or equal to upper endpoint ({1})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		MAP_MODIFIED_WHILE_ITERATING("map has been modified while iterating"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		EVALUATIONS("evaluations"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		MAX_COUNT_EXCEEDED("maximal count ({0}) exceeded"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		MAX_ITERATIONS_EXCEEDED("maximal number of iterations ({0}) exceeded"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		MINIMAL_STEPSIZE_REACHED_DURING_INTEGRATION("minimal step size ({1,number,0.00E00}) reached, integration needs {0,number,0.00E00}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		MISMATCHED_LOESS_ABSCISSA_ORDINATE_ARRAYS("Loess expects the abscissa and ordinate arrays to be of the same size, but got {0} abscissae and {1} ordinatae"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		MUTATION_RATE("mutation rate ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NAN_ELEMENT_AT_INDEX("element {0} is NaN"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NAN_VALUE_CONVERSION("cannot convert NaN value"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NEGATIVE_BRIGHTNESS_EXPONENT("brightness exponent should be positive or null, but got {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NEGATIVE_COMPLEX_MODULE("negative complex module {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NEGATIVE_ELEMENT_AT_2D_INDEX("element ({0}, {1}) is negative: {2}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NEGATIVE_ELEMENT_AT_INDEX("element {0} is negative: {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NEGATIVE_NUMBER_OF_SUCCESSES("number of successes must be non-negative ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NUMBER_OF_SUCCESSES("number of successes ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NEGATIVE_NUMBER_OF_TRIALS("number of trials must be non-negative ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NUMBER_OF_INTERPOLATION_POINTS("number of interpolation points ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NUMBER_OF_TRIALS("number of trials ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_CONVEX("vertices do not form a convex hull in CCW winding"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ROBUSTNESS_ITERATIONS("number of robustness iterations ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		START_POSITION("start position ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NON_CONVERGENT_CONTINUED_FRACTION("Continued fraction convergents failed to converge (in less than {0} iterations) for value {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NON_INVERTIBLE_TRANSFORM("non-invertible affine transform collapses some lines into single points"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NON_POSITIVE_MICROSPHERE_ELEMENTS("number of microsphere elements must be positive, but got {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NON_POSITIVE_POLYNOMIAL_DEGREE("polynomial degree must be positive: degree={0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NON_REAL_FINITE_ABSCISSA("all abscissae must be finite real numbers, but {0}-th is {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NON_REAL_FINITE_ORDINATE("all ordinatae must be finite real numbers, but {0}-th is {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NON_REAL_FINITE_WEIGHT("all weights must be finite real numbers, but {0}-th is {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NON_SQUARE_MATRIX("non square ({0}x{1}) matrix"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NORM("Norm ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NORMALIZE_INFINITE("Cannot normalize to an infinite value"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NORMALIZE_NAN("Cannot normalize to NaN"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_ADDITION_COMPATIBLE_MATRICES("{0}x{1} and {2}x{3} matrices are not addition compatible"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_DECREASING_NUMBER_OF_POINTS("points {0} and {1} are not decreasing ({2} < {3})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_DECREASING_SEQUENCE("points {3} and {2} are not decreasing ({1} < {0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_ENOUGH_DATA_FOR_NUMBER_OF_PREDICTORS("not enough data ({0} rows) for this many predictors ({1} predictors)"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_ENOUGH_POINTS_IN_SPLINE_PARTITION("spline partition must have at least {0} points, got {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_INCREASING_NUMBER_OF_POINTS("points {0} and {1} are not increasing ({2} > {3})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_INCREASING_SEQUENCE("points {3} and {2} are not increasing ({1} > {0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_MULTIPLICATION_COMPATIBLE_MATRICES("{0}x{1} and {2}x{3} matrices are not multiplication compatible"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POSITIVE_DEFINITE_MATRIX("not positive definite matrix"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NON_POSITIVE_DEFINITE_MATRIX("not positive definite matrix: diagonal element at ({1},{1}) is smaller than {2} ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NON_POSITIVE_DEFINITE_OPERATOR("non positive definite linear operator"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NON_SELF_ADJOINT_OPERATOR("non self-adjoint linear operator"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NON_SQUARE_OPERATOR("non square ({0}x{1}) linear operator"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		DEGREES_OF_FREEDOM("degrees of freedom ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POSITIVE_DEGREES_OF_FREEDOM("degrees of freedom must be positive ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POSITIVE_ELEMENT_AT_INDEX("element {0} is not positive: {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POSITIVE_EXPONENT("invalid exponent {0} (must be positive)"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NUMBER_OF_ELEMENTS_SHOULD_BE_POSITIVE("number of elements should be positive ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		BASE("base ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		EXPONENT("exponent ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POSITIVE_LENGTH("length must be positive ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		LENGTH("length ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POSITIVE_MEAN("mean must be positive ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		MEAN("mean ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POSITIVE_NUMBER_OF_SAMPLES("number of sample is not positive: {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NUMBER_OF_SAMPLES("number of samples ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POSITIVE_PERMUTATION("permutation k ({0}) must be positive"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		PERMUTATION_SIZE("permutation size ({0}"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POSITIVE_POISSON_MEAN("the Poisson mean must be positive ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POSITIVE_POPULATION_SIZE("population size must be positive ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		POPULATION_SIZE("population size ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POSITIVE_ROW_DIMENSION("invalid row dimension: {0} (must be positive)"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POSITIVE_SAMPLE_SIZE("sample size must be positive ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POSITIVE_SCALE("scale must be positive ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		SCALE("scale ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POSITIVE_SHAPE("shape must be positive ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		SHAPE("shape ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POSITIVE_STANDARD_DEVIATION("standard deviation must be positive ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		STANDARD_DEVIATION("standard deviation ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POSITIVE_UPPER_BOUND("upper bound must be positive ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POSITIVE_WINDOW_SIZE("window size must be positive ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POWER_OF_TWO("{0} is not a power of 2"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POWER_OF_TWO_CONSIDER_PADDING("{0} is not a power of 2, consider padding for fix"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_POWER_OF_TWO_PLUS_ONE("{0} is not a power of 2 plus one"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_STRICTLY_DECREASING_NUMBER_OF_POINTS("points {0} and {1} are not strictly decreasing ({2} <= {3})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_STRICTLY_DECREASING_SEQUENCE("points {3} and {2} are not strictly decreasing ({1} <= {0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_STRICTLY_INCREASING_KNOT_VALUES("knot values must be strictly increasing"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_STRICTLY_INCREASING_NUMBER_OF_POINTS("points {0} and {1} are not strictly increasing ({2} >= {3})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_STRICTLY_INCREASING_SEQUENCE("points {3} and {2} are not strictly increasing ({1} >= {0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_SUBTRACTION_COMPATIBLE_MATRICES("{0}x{1} and {2}x{3} matrices are not subtraction compatible"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_SUPPORTED_IN_DIMENSION_N("method not supported in dimension {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NOT_SYMMETRIC_MATRIX("not symmetric matrix"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NON_SYMMETRIC_MATRIX("non symmetric matrix: the difference between entries at ({0},{1}) and ({1},{0}) is larger than {2}"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NO_BIN_SELECTED("no bin selected"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NO_CONVERGENCE_WITH_ANY_START_POINT("none of the {0} start points lead to convergence"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NO_DATA("no data"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NO_DEGREES_OF_FREEDOM("no degrees of freedom ({0} measurements, {1} parameters)"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NO_DENSITY_FOR_THIS_DISTRIBUTION("This distribution does not have a density function implemented"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NO_FEASIBLE_SOLUTION("no feasible solution"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NO_OPTIMUM_COMPUTED_YET("no optimum computed yet"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NO_REGRESSORS("Regression model must include at least one regressor"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NO_RESULT_AVAILABLE("no result available"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NO_SUCH_MATRIX_ENTRY("no entry at indices ({0}, {1}) in a {2}x{3} matrix"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NAN_NOT_ALLOWED("NaN is not allowed"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NULL_NOT_ALLOWED("null is not allowed"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ARRAY_ZERO_LENGTH_OR_NULL_NOT_ALLOWED("a null or zero length array not allowed"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		COVARIANCE_MATRIX("covariance matrix"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		DENOMINATOR("denominator"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		DENOMINATOR_FORMAT("denominator format"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		FRACTION("fraction"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		FUNCTION("function"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		IMAGINARY_FORMAT("imaginary format"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		INPUT_ARRAY("input array"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NUMERATOR("numerator"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NUMERATOR_FORMAT("numerator format"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		OBJECT_TRANSFORMATION("conversion exception in transformation"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		REAL_FORMAT("real format"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		WHOLE_FORMAT("whole format"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NUMBER_TOO_LARGE("{0} is larger than the maximum ({1})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NUMBER_TOO_SMALL("{0} is smaller than the minimum ({1})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NUMBER_TOO_LARGE_BOUND_EXCLUDED("{0} is larger than, or equal to, the maximum ({1})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NUMBER_TOO_SMALL_BOUND_EXCLUDED("{0} is smaller than, or equal to, the minimum ({1})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NUMBER_OF_SUCCESS_LARGER_THAN_POPULATION_SIZE("number of successes ({0}) must be less than or equal to population size ({1})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NUMERATOR_OVERFLOW_AFTER_MULTIPLY("overflow, numerator too large after multiply: {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		N_POINTS_GAUSS_LEGENDRE_INTEGRATOR_NOT_SUPPORTED("{0} points Legendre-Gauss integrator not supported, number of points must be in the {1}-{2} range"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		OBSERVED_COUNTS_ALL_ZERO("observed counts are all 0 in observed array {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		OBSERVED_COUNTS_BOTTH_ZERO_FOR_ENTRY("observed counts are both zero for entry {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		BOBYQA_BOUND_DIFFERENCE_CONDITION("the difference between the upper and lower bound must be larger than twice the initial trust region radius ({0})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		OUT_OF_BOUNDS_QUANTILE_VALUE("out of bounds quantile value: {0}, must be in (0, 100]"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		OUT_OF_BOUNDS_CONFIDENCE_LEVEL("out of bounds confidence level {0}, must be between {1} and {2}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		OUT_OF_BOUND_SIGNIFICANCE_LEVEL("out of bounds significance level {0}, must be between {1} and {2}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		SIGNIFICANCE_LEVEL("significance level ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		OUT_OF_ORDER_ABSCISSA_ARRAY("the abscissae array must be sorted in a strictly increasing order, but the {0}-th element is {1} whereas {2}-th is {3}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		OUT_OF_RANGE_ROOT_OF_UNITY_INDEX("out of range root of unity index {0} (must be in [{1};{2}])"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		OUT_OF_RANGE("out of range"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		OUT_OF_RANGE_SIMPLE("{0} out of [{1}, {2}] range"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		OUT_OF_RANGE_LEFT("{0} out of ({1}, {2}] range"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		OUT_OF_RANGE_RIGHT("{0} out of [{1}, {2}) range"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		OUTLINE_BOUNDARY_LOOP_OPEN("an outline boundary loop is open"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		OVERFLOW("overflow"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		OVERFLOW_IN_FRACTION("overflow in fraction {0}/{1}, cannot negate"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		OVERFLOW_IN_ADDITION("overflow in addition: {0} + {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		OVERFLOW_IN_SUBTRACTION("overflow in subtraction: {0} - {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		PERCENTILE_IMPLEMENTATION_CANNOT_ACCESS_METHOD("cannot access {0} method in percentile implementation {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		PERCENTILE_IMPLEMENTATION_UNSUPPORTED_METHOD("percentile implementation {0} does not support {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		PERMUTATION_EXCEEDS_N("permutation size ({0}) exceeds permuation domain ({1})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		POLYNOMIAL("polynomial"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		POLYNOMIAL_INTERPOLANTS_MISMATCH_SEGMENTS("number of polynomial interpolants must match the number of segments ({0} != {1} - 1)"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		POPULATION_LIMIT_NOT_POSITIVE("population limit has to be positive"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		POWER_NEGATIVE_PARAMETERS("cannot raise an integral value to a negative power ({0}^{1})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		PROPAGATION_DIRECTION_MISMATCH("propagation direction mismatch"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		RANDOMKEY_MUTATION_WRONG_CLASS("RandomKeyMutation works only with RandomKeys, not {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ROOTS_OF_UNITY_NOT_COMPUTED_YET("roots of unity have not been computed yet"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ROTATION_MATRIX_DIMENSIONS("a {0}x{1} matrix cannot be a rotation matrix"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ROW_INDEX_OUT_OF_RANGE("row index {0} out of allowed range [{1}, {2}]"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ROW_INDEX("row index ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		SAME_SIGN_AT_ENDPOINTS("function values at endpoints do not have different signs, endpoints: [{0}, {1}], values: [{2}, {3}]"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		SAMPLE_SIZE_EXCEEDS_COLLECTION_SIZE("sample size ({0}) exceeds collection size ({1})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		SAMPLE_SIZE_LARGER_THAN_POPULATION_SIZE("sample size ({0}) must be less than or equal to population size ({1})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		SIMPLEX_NEED_ONE_POINT("simplex must contain at least one point"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		SIMPLE_MESSAGE("{0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		SINGULAR_MATRIX("matrix is singular"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		SINGULAR_OPERATOR("operator is singular"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		SUBARRAY_ENDS_AFTER_ARRAY_END("subarray ends after array end"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		TOO_LARGE_CUTOFF_SINGULAR_VALUE("cutoff singular value is {0}, should be at most {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		TOO_LARGE_TOURNAMENT_ARITY("tournament arity ({0}) cannot be bigger than population size ({1})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		TOO_MANY_ELEMENTS_TO_DISCARD_FROM_ARRAY("cannot discard {0} elements from a {1} elements array"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		TOO_MANY_REGRESSORS("too many regressors ({0}) specified, only {1} in the model"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		TOO_SMALL_COST_RELATIVE_TOLERANCE("cost relative tolerance is too small ({0}), no further reduction in the sum of squares is possible"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		TOO_SMALL_INTEGRATION_INTERVAL("too small integration interval: length = {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		TOO_SMALL_ORTHOGONALITY_TOLERANCE("orthogonality tolerance is too small ({0}), solution is orthogonal to the jacobian"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		TOO_SMALL_PARAMETERS_RELATIVE_TOLERANCE("parameters relative tolerance is too small ({0}), no further improvement in the approximate solution is possible"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		TRUST_REGION_STEP_FAILED("trust region step has failed to reduce Q"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		TWO_OR_MORE_CATEGORIES_REQUIRED("two or more categories required, got {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		TWO_OR_MORE_VALUES_IN_CATEGORY_REQUIRED("two or more values required in each category, one has {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		UNABLE_TO_BRACKET_OPTIMUM_IN_LINE_SEARCH("unable to bracket optimum in line search"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		UNABLE_TO_COMPUTE_COVARIANCE_SINGULAR_PROBLEM("unable to compute covariances: singular problem"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		UNABLE_TO_FIRST_GUESS_HARMONIC_COEFFICIENTS("unable to first guess the harmonic coefficients"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		UNABLE_TO_ORTHOGONOLIZE_MATRIX("unable to orthogonalize matrix in {0} iterations"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		UNABLE_TO_PERFORM_QR_DECOMPOSITION_ON_JACOBIAN("unable to perform Q.R decomposition on the {0}x{1} jacobian matrix"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		UNABLE_TO_SOLVE_SINGULAR_PROBLEM("unable to solve: singular problem"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		UNBOUNDED_SOLUTION("unbounded solution"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		UNKNOWN_MODE("unknown mode {0}, known modes: {1} ({2}), {3} ({4}), {5} ({6}), {7} ({8}), {9} ({10}) and {11} ({12})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		UNKNOWN_PARAMETER("unknown parameter {0}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		UNMATCHED_ODE_IN_EXPANDED_SET("ode does not match the main ode set in the extended set"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CANNOT_PARSE_AS_TYPE("string \"{0}\" unparseable (from position {1}) as an object of type {2}"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		CANNOT_PARSE("string \"{0}\" unparseable (from position {1})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		UNPARSEABLE_3D_VECTOR("unparseable 3D vector: \"{0}\""),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		UNPARSEABLE_COMPLEX_NUMBER("unparseable complex number: \"{0}\""),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		UNPARSEABLE_REAL_VECTOR("unparseable real vector: \"{0}\""),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		UNSUPPORTED_EXPANSION_MODE("unsupported expansion mode {0}, supported modes are {1} ({2}) and {3} ({4})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		UNSUPPORTED_OPERATION("unsupported operation"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ARITHMETIC_EXCEPTION("arithmetic exception"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ILLEGAL_STATE("illegal state"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		USER_EXCEPTION("exception generated in user code"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		URL_CONTAINS_NO_DATA("URL {0} contains no data"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		VALUES_ADDED_BEFORE_CONFIGURING_STATISTIC("{0} values have been added before statistic is configured"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		VECTOR_LENGTH_MISMATCH("vector length mismatch: got {0} but expected {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		VECTOR_MUST_HAVE_AT_LEAST_ONE_ELEMENT("vector must have at least one element"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		WEIGHT_AT_LEAST_ONE_NON_ZERO("weigth array must contain at least one non-zero value"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		WRONG_BLOCK_LENGTH("wrong array shape (block length = {0}, expected {1})"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		WRONG_NUMBER_OF_POINTS("{0} points are required, got only {1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		NUMBER_OF_POINTS("number of points ({0})"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ZERO_DENOMINATOR("denominator must be different from 0"), // keep
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ZERO_DENOMINATOR_IN_FRACTION("zero denominator in fraction {0}/{1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ZERO_FRACTION_TO_DIVIDE_BY("the fraction to divide by must not be zero: {0}/{1}"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ZERO_NORM("zero norm"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ZERO_NORM_FOR_ROTATION_AXIS("zero norm for rotation axis"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ZERO_NORM_FOR_ROTATION_DEFINING_VECTOR("zero norm for rotation defining vector"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		ZERO_NOT_ALLOWED("zero not allowed here");

		// CHECKSTYLE: resume JavadocVariable
		// CHECKSTYLE: resume MultipleVariableDeclarations


		/// <summary>
		/// Source English format. </summary>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain fields in .NET:
//		private final String sourceFormat;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="sourceFormat"> source English format to use when no
		/// localized version is available </param>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain methods in .NET:
//		private LocalizedFormats(final String sourceFormat)
	//	{
	//		this.sourceFormat = sourceFormat;
	//	}

		/// <summary>
		/// {@inheritDoc} </summary>

		/// <summary>
		/// {@inheritDoc} </summary>

	}
	public static partial class EnumExtensionMethods
	{
		public static string getSourceString(this LocalizedFormats instanceJavaToDotNetTempPropertyGetSourceString)
		{
			return sourceFormat;
		}
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static String getLocalizedString(final java.util.Locale locale)
		public static string getLocalizedString(this LocalizedFormats instance, Locale locale)
		{
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String path = LocalizedFormats.class.getName().replaceAll("\\.", "/");
				string path = typeof(LocalizedFormats).Name.replaceAll("\\.", "/");
				ResourceBundle bundle = ResourceBundle.getBundle("assets/" + path, locale);
				if (bundle.Locale.Language.Equals(locale.Language))
				{
					// the value of the resource is the translated format
					return bundle.getString(ToString());
				}

			} // NOPMD
			catch (MissingResourceException mre)
			{
				// do nothing here
			}

			// either the locale is not supported or the resource is unknown
			// don't translate and fall back to using the source format
			return sourceFormat;

		}
	}

}