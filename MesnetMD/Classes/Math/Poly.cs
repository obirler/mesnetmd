﻿/*
========================================================================
    Copyright (C) 2016 Omer Birler.
    
    This file is part of Mesnet.

    Mesnet is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Mesnet is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Mesnet.  If not, see <http://www.gnu.org/licenses/>.
========================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using MesnetMD.Classes.Tools;

namespace MesnetMD.Classes.Math
{
    /// <summary>
    /// 
    /// </summary>
    public class Poly
    {
        #region Constructor Overloading:
        /// <summary>
        /// Constructor which Read String and find Terms in it. Create new Term for each
        /// Term String and add it to the Terms Collection. 
        /// </summary>
        /// <param name="PolyExpression"></param>
        public Poly(string PolyExpression)
        {
            this._Terms = new TermCollection();
            this.ReadPolyExpression(PolyExpression);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Poly"/> class from a string expression and between start point and endpoint.
        /// </summary>
        /// <param name="PolyExpression">The poly expression string.</param>
        /// <param name="startpoint">The startpoint of the polynomial.</param>
        /// <param name="endpoint">The endpoint point of the poynomial.</param>
        public Poly(string PolyExpression, double startpoint, double endpoint)
        {
            this._Terms = new TermCollection();
            this.ReadPolyExpression(PolyExpression);
            _startpoint = startpoint;
            _endpoint = endpoint;
        }

        /// <summary>
        /// Constructor which create a new instance of Poly with a predefined TermCollection.
        /// </summary>
        /// <param name="terms"></param>
        public Poly(TermCollection terms)
        {
            this.Terms = terms;
            this.Terms.Sort(TermCollection.SortType.ASC);
        }

        public Poly(TermCollection terms, double startpoint, double endpoint)
        {
            this.Terms = terms;
            this.Terms.Sort(TermCollection.SortType.ASC);
            _startpoint = startpoint;
            _endpoint = endpoint;
        }

        public Poly(Term term, double startpoint, double endpoint)
        {
            var terms = new TermCollection();
            terms.Add(term);
            this.Terms = terms;
            this.Terms.Sort(TermCollection.SortType.ASC);
            _startpoint = startpoint;
            _endpoint = endpoint;
        }

        public Poly()
        {           
        }
        #endregion

        #region Destructor:
        /// <summary>
        /// Clear the Term Collections
        /// </summary>
        ~Poly()
        {
            if (Terms != null)
            {
                Terms.Clear();
            }
        }

        #endregion 

        #region Override methods:

        /// <summary>
        /// This will print out the string format of polynomial by calling each term in the collection.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = string.Empty;

            if (this.Terms != null)
            {
                this.Terms.Sort(TermCollection.SortType.DES);

                if (Terms.Count > 0)
                {
                    foreach (Term t in this.Terms)
                    {
                        result += t.ToString();
                    }
                    if (result.Substring(0, 1) == "+")
                        result = result.Remove(0, 1);
                }
                else
                {
                    result = "0";
                }
            }
            else
            {
                result = "0";
            }

            return result;
        }

        /// <summary>
        /// Creates the polynomial string which all coefficient rounded to given digit
        /// </summary>
        /// <param name="digit">The desired digit to be rounded.</param>
        /// <returns></returns>
        public string GetString(int digit)
        {
            this.Terms.Sort(TermCollection.SortType.DES);

            string result = string.Empty;

            if (Terms.Count > 0)
            {
                foreach (Term t in this.Terms)
                {
                    result += t.GetString(digit);
                }
                if (result.Substring(0, 1) == "+")
                    result = result.Remove(0, 1);
            }
            else
            {
                result = "0";
            }

            return result;
        }

        #endregion

        #region Methods:

        public void Parse(string PolyExpression)
        {
            this._Terms = new TermCollection();
            this.ReadPolyExpression(PolyExpression);
        }

        /// <summary>
        /// Calculate the Value of Polynomial with the given X value.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Calculate(double x)
        {
            double result = 0;
            foreach (Term t in this.Terms)
            {
                result += t.Coefficient *System.Math.Pow(x, t.Power);
            }
            return result;
        }

        /// <summary>
        /// Finds the maximum value of the polynomial using numerical method.
        /// </summary>
        /// <param name="startpoint">The start point of the calculation.</param>
        /// <param name="endpoint">The end point of the calculation.</param>
        /// <param name="digit">The desired digit that will be rounded.</param>
        /// <returns>The maximum value of the polynomial</returns>
        public double Maximum(double startpoint, double endpoint)
        {
            var diff = 1 / 200.0;

            double left;
            double right;
            double max = Double.MinValue;
            double value;
            double maxindex = 0;

            for (double i = _startpoint; System.Math.Round(i - _endpoint, 10) <= 0; i = i + diff)
            {
                
                value = Calculate(i);
                if (value > max)
                {
                    max = value;
                    maxindex = i;
                }
            }

            left = maxindex - diff;

            right = maxindex + diff;

            if (left < _startpoint || right > _endpoint)
            {
                return max;
            }

            diff = (right - left) / 100.0;

            for (double i = left; i <= right; i = i + diff)
            {
                value = Calculate(i);
                if (value > max)
                {
                    max = value;
                }
            }

            if (max < Calculate(startpoint))
            {
                max = Calculate(startpoint);
            }

            if (max < Calculate(endpoint))
            {
                max = Calculate(endpoint);
            }

            return max;
        }

        public double Maximum()
        {
            return Maximum(_startpoint, _endpoint);
        }

        public double MaximumAbs(double initialstep, double endstep, int digit = 4)
        {
            if (digit < 0)
            {
                digit = 0;
            }

            var diff = 1 / initialstep;

            double left;
            double right;
            double absmax = Double.MinValue;
            double value;
            double maxindex = 0;

            for (double i = _startpoint; i < _endpoint; i = i + diff)
            {
                value = System.Math.Abs(Calculate(i));
                if (value > absmax)
                {
                    absmax = value;
                    maxindex = i;
                }
            }

            left = maxindex - diff;

            right = maxindex + diff;

            if (left < _startpoint || right > _endpoint)
            {
                return System.Math.Round(absmax, digit);
            }

            diff = (right - left) / endstep;

            for (double i = left; i < right; i = i + diff)
            {
                value = System.Math.Abs(Calculate(i));
                if (value > absmax)
                {
                    absmax = value;
                }
            }

            return System.Math.Round(absmax, digit);
        }

        public double MaximumAbs()
        {
            return MaximumAbs(200.0, 100.0, 4);
        }

        public double MaxLocation(double startpoint, double endpoint, int digit = 4)
        {
            if (digit < 0)
            {
                digit = 0;
            }

            var diff = 1 / 200.0;

            double left;
            double right;
            double max = Double.MinValue;
            double value;
            double maxindex = 0;

            for (double i = _startpoint; System.Math.Round(i - _endpoint, 10) <= 0; i = i + diff)
            {
                value = Calculate(i);
                if (value > max)
                {
                    max = value;
                    maxindex = i;
                }
            }

            left = maxindex - diff;

            right = maxindex + diff;

            if (left < _startpoint || right > _endpoint)
            {
                return System.Math.Round(maxindex, digit);
            }

            diff = (right - left) / 100.0;

            for (double i = left; i <= right; i = i + diff)
            {
                value = Calculate(i);
                if (value > max)
                {
                    max = value;
                    maxindex = i;
                }
            }

            if (max < Calculate(startpoint))
            {
                max = Calculate(startpoint);
                maxindex = startpoint;
            }
            if (max < Calculate(endpoint))
            {
                max = Calculate(endpoint);
                maxindex = endpoint;
            }

            return System.Math.Round(maxindex, digit);
        }

        public double MaxLocation(int digit = 4)
        {
            return MaxLocation(_startpoint, _endpoint, digit);
        }

        /// <summary>
        /// Return minimum value of the polynomial in given interval. The min value rounded to the desired digit.
        /// </summary>
        /// <param name="startpoint">The startpoint of x for minimum scan.</param>
        /// <param name="endpoint">The endpoint of x for minimum scan.</param>
        /// <param name="digit">The desired number of decimals to round.</param>
        /// <returns></returns>
        public double Minimum(double startpoint, double endpoint, int digit = 4)
        {
            if (digit < 0)
            {
                digit = 0;
            }

            var diff = 1 / 200.0;

            double left;
            double right;
            double min = Double.MaxValue;
            double value;
            double minindex = 0;

            for (double i = _startpoint; System.Math.Round(i - _endpoint, 10) <= 0; i = i + diff)
            {
                value = Calculate(i);
                if (value < min)
                {
                    min = value;
                    minindex = i;
                }
            }

            left = minindex - diff;

            right = minindex + diff;

            if (left < _startpoint || right > _endpoint)
            {
                return System.Math.Round(min, digit);
            }

            diff = (right - left) / 100.0;

            for (double i = left; i <= right; i = i + diff)
            {
                value = Calculate(i);
                if (value < min)
                {
                    min = value;
                }
            }

            if (min > Calculate(startpoint))
            {
                min = Calculate(startpoint);
            }

            if (min > Calculate(endpoint))
            {
                min = Calculate(endpoint);
            }

            return System.Math.Round(min, digit);
        }

        public double Minimum()
        {
            return Minimum(_startpoint, _endpoint);
        }

        public double MinLocation(double startpoint, double endpoint, int digit = 4)
        {
            if (digit < 0)
            {
                digit = 0;
            }

            var diff = 1 / 200.0;

            double left;
            double right;
            double min = Double.MaxValue;
            double value;
            double minindex = 0;

            for (double i = _startpoint; System.Math.Round(i - _endpoint, 10) <= 0; i = i + diff)
            {
                value = Calculate(i);
                if (value < min)
                {
                    min = value;
                    minindex = i;
                }
            }

            left = minindex - diff;

            right = minindex + diff;

            if (left < _startpoint || right > _endpoint)
            {
                return System.Math.Round(minindex, digit);
            }

            diff = (right - left) / 100.0;

            for (double i = left; i < right; i = i + diff)
            {
                value = Calculate(i);
                if (value < min)
                {
                    min = value;
                    minindex = i;
                }
            }

            return System.Math.Round(minindex, digit);
        }

        public double MinLocation(int digit = 4)
        {
            return MinLocation(_startpoint, _endpoint, digit);
        }

        /// <summary>
        /// Static method which Validate the input Expression
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool ValidateExpression(string Expression)
        {
            if (Expression.Length == 0)
                return false;

            Expression = Expression.Trim();
            Expression = Expression.Replace(" ", "");
            while (Expression.IndexOf("--") > -1 | Expression.IndexOf("++") > -1 | Expression.IndexOf("^^") > -1 | Expression.IndexOf("xx") > -1)
            {
                Expression = Expression.Replace("--", "-");
                Expression = Expression.Replace("++", "+");
                Expression = Expression.Replace("^^", "^");
                Expression = Expression.Replace("xx", "x");
            }
            string ValidChars = "+-x1234567890^.E";
            bool result = true;
            foreach (char c in Expression)
            {
                if (ValidChars.IndexOf(c) == -1)
                {
                    result = false;
                } 
            }
            return result;
        }

        /// <summary>
        /// Read Method will Identify any Term in the Expression and Create a new Instance of 
        /// Term Class and add it to the TermCollection
        /// </summary>
        /// <param name="PolyExpression">input string of Polynomial Expression</param>
        private void ReadPolyExpression(string PolyExpression)
        {
            if (ValidateExpression(PolyExpression))
            {
                PolyExpression = PolyExpression.Replace(" ", "");
                string NextChar = string.Empty;
                string NextTerm = string.Empty;
                bool epow = false;
                for (int i = 0; i < PolyExpression.Length; i++)
                {
                    NextChar = PolyExpression.Substring(i, 1);
                    if (NextChar == "E")
                    {
                        epow = true;
                    }

                    if ((NextChar == "-" | NextChar == "+") & i > 0)
                    {
                        if (epow)
                        {
                            epow = false;
                        }
                        else
                        {
                            handleterm(NextTerm);
                            NextTerm = string.Empty;
                        }                   
                    }
                    NextTerm += NextChar;
                }
                handleterm(NextTerm);

                this.Terms.Sort(TermCollection.SortType.ASC);
            }
            else
            {
                MesnetMDDebug.WriteError("Invalid Polynomial Expression : " + PolyExpression);
                throw new Exception("Invalid Polynomial Expression");
            }
        }

        private void handleterm(string term)
        {
            Term termitem;
            if (term.Contains("E"))
            {
                var coeffs = term.Split('E');
                double c = Convert.ToDouble(coeffs[0]);
                if (coeffs[1].Contains("x^"))
                {
                    var epxs= coeffs[1].Split(new string[] { "x^" }, StringSplitOptions.None);
                    double p1 = Convert.ToDouble(epxs[0]);
                    double p2 = Convert.ToDouble(epxs[1]);
                    termitem = new Term();
                    termitem.Coefficient = c * System.Math.Pow(10, p1);
                    termitem.Power = p2;
                    Terms.Add(termitem);
                }
                else if(coeffs[1].Contains("x"))
                {
                    var epxs = coeffs[1].Split(new string[] { "x" }, StringSplitOptions.None);
                    double p1 = Convert.ToDouble(epxs[0]);
                    termitem = new Term();
                    termitem.Coefficient = c * System.Math.Pow(10, p1);
                    termitem.Power = 1;
                    Terms.Add(termitem);
                }
                else
                {
                    double p = Convert.ToDouble(coeffs[1]);
                    var termItem = new Term();
                    termItem.Coefficient = c * System.Math.Pow(10, p);
                    Terms.Add(termItem);
                }                
            }
            else
            {
                termitem = new Term(term);
                Terms.Add(termitem);
            }
        }

        public Poly Integrate()
        {
            var terms = new TermCollection();
            foreach (Term t in this.Terms)
            {
                var pow = t.Power + 1;
                var coeff = t.Coefficient / (t.Power + 1);
                terms.Add(new Term(pow, coeff));
            }
            return new Poly(terms);
        }

        /// <summary>
        /// Calculates the definite integral with given range.
        /// </summary>
        /// <param name="start">Start value of integral.</param>
        /// <param name="end">End of the integral.</param>
        /// <returns></returns>
        public double DefiniteIntegral(double start, double end)
        {
            double result = 0;

            Poly integral = Integrate();

            result = integral.Calculate(end) - integral.Calculate(start);

            return result;
        }

        public double DefiniteIntegral()
        {
            return DefiniteIntegral(StartPoint, EndPoint);
        }

        public List<KeyValuePair<double, double>> CalculateMagnitudeAndLocation()
        {
            var roots = Roots();
            var dict = new List<KeyValuePair<double, double>>();
            double magnitude;
            double location;
            if (roots != null)
            {
                for (var i = 0; i < roots.Count; i++)
                {
                    if (i == 0)
                    {
                        magnitude = DefiniteIntegral(StartPoint, roots[i]);
                        location = LoadCenter(StartPoint, roots[i]);
                        dict.Add(new KeyValuePair<double, double>(location, magnitude));
                    }
                    else
                    {
                        magnitude = DefiniteIntegral(roots[i - 1], roots[i]);
                        location = LoadCenter(roots[i - 1], roots[i]);
                        dict.Add(new KeyValuePair<double, double>(location, magnitude));
                    }
                }
                magnitude = DefiniteIntegral(roots.Last(), EndPoint);
                location = LoadCenter(roots.Last(), EndPoint);
                dict.Add(new KeyValuePair<double, double>(location, magnitude));
            }
            else
            {
                magnitude = DefiniteIntegral();
                location = LoadCenter();
                dict.Add(new KeyValuePair<double, double>(location, magnitude));
            }

            return dict;
        }
      
        public Poly Derivate()
        {
            var terms = new TermCollection();
            foreach (Term t in this.Terms)
            {
                var pow = t.Power - 1;
                var coeff = t.Coefficient * t.Power;
                terms.Add(new Term(pow, coeff));
            }

            return new Poly(terms);
        }

        /// <summary>
        /// Finds the center of the polynom on x-axis in the range given.
        /// </summary>
        /// <param name="start">The start point in x-axis of the polynom.</param>
        /// <param name="end">The end point in x-axis of the polynom.</param>
        /// <returns>The x-axis of center of area of the polynom.</returns>
        public double LoadCenter(double start, double end)
        {
            double centerpoint = 0;

            Poly p1 = new Poly("x");

            Poly p2 = p1 * this;

            centerpoint = p2.DefiniteIntegral(start, end)/this.DefiniteIntegral(start, end);

            return centerpoint;
        }

        public double LoadCenter()
        {
            return LoadCenter(StartPoint, EndPoint);
        }

        /// <summary>
        /// Conjugates polynomial with the specified length. Conjugating is basically converting x in polynomial into (length-x).
        /// </summary>
        /// <param name="x">The desired conjugation length</param>
        /// <returns>Conjugated polynomial</returns>
        public ConjugatePoly Conjugate(double length)
        {
            var terms = new ConjugateTermCollection();
            foreach (Term t in this.Terms)
            {
                if (t.Power > 0)
                {
                    string newcoeff;
                    if (t.Coefficient == 1)
                    {
                        newcoeff = "(" + length + "-x)";
                    }
                    else if (t.Coefficient == -1)
                    {
                        newcoeff= "-(" + length + "-x)";
                    }
                    else
                    {
                       newcoeff = t.Coefficient + "(" + length + "-x)";
                    }
                    var newterm = new ConjugateTerm(t.Power, newcoeff);
                    terms.Add(newterm);
                }
                else if (t.Power == 0)
                {
                    var newterm = new ConjugateTerm(t.Power, t.Coefficient.ToString());
                    terms.Add(newterm);
                }               
            }
            var conjugatepoly = new ConjugatePoly(terms);
            conjugatepoly.StartPoint = length - EndPoint;
            conjugatepoly.EndPoint = length - StartPoint;
            return conjugatepoly;
        }

        /// <summary>
        /// Propagates polynomial with the specified length. Propagating is basically the same as conjugating but all the term 
        /// in the polynomial are expanded and created a polynomial with more terms naturally. To propagate a polynamial, it shouldn't
        /// have any non-integer-powered terms
        /// </summary>
        /// <param name="x">The desired propagation length</param>
        /// <returns>Propagated polynomial</returns>
        public Poly Propagate(double length)
        {
            //Basically expand a(L-x)^n terms
            var terms = new TermCollection();
            foreach (Term t in this.Terms)
            {
                if (t.Power > 0)
                {
                    int n = (int)t.Power;

                    for (int i = 0; i <= n; i++)
                    {
                        var newterm = new Term();
                        newterm.Coefficient = t.Coefficient*Algebra.Combination(n, i) * System.Math.Pow(-1, i) *
                                           System.Math.Pow(length, n - i);
                        newterm.Power = i;
                        terms.Add(newterm);
                    }
                }
                else if (t.Power == 0)
                {
                    var newterm = new Term(t.Power, t.Coefficient);
                    terms.Add(newterm);
                }
            }
            var newpoly = new Poly(terms);
            newpoly.StartPoint = length - EndPoint;
            newpoly.EndPoint = length - StartPoint;
            return newpoly;
        }

        public Poly Cube()
        {
            var returnpoly = this * this * this;
            returnpoly.StartPoint = this.StartPoint;
            returnpoly.EndPoint = this.EndPoint;
            return returnpoly;
        }

        /// <summary>
        /// Calculates all root locations
        /// </summary>
        /// <returns>List of root locations</returns>
        public List<double> Roots()
        {
            //We are using this step which means we check sign change at interval of 1 centimeter along the beam
            double step = 0.01;

            Poly derivative = Derivate();

            var rootlist = new List<double>();
            double previous = 0;

            for (double i = StartPoint; i <= EndPoint; i=i+step)
            {
                if (i > EndPoint)
                {
                    i = EndPoint;
                }
                if (i > StartPoint)
                {
                    if (previous > 0)
                    {
                        if (Calculate(i) < 0)
                        {
                            //there is a sign change and root so find it
                            double root = findroot(i - step, i, derivative);
                            rootlist.Add(root);
                        }
                    }
                    else if(previous < 0)
                    {
                        if (Calculate(i) > 0)
                        {
                            //there is a sign change and root so find it
                            double root = findroot(i - step, i, derivative);
                            rootlist.Add(root);
                        }
                    }
                }
                previous = Calculate(i);
            }

            if (rootlist.Count > 0)
            {
                return rootlist;
            }
            return null;
        }

        /// <summary>
        /// Calculates degree of the polynomial. Degree is the highest power of all terms
        /// </summary>
        /// <returns>Degree of the polynomial</returns>
        public double Degree()
        {
            double value = Double.MinValue;
            foreach (Term term in _Terms)
            {
                if (term.Power > value)
                {
                    value = term.Power;
                }
            }
            return value;
        }

        public bool IsConstant()
        {
            if (Terms.Count == 1)
            {
                if (Terms[0].Power == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsLinear()
        {
            if (Terms.Count > 2)
            {
                return false;
            }

            foreach (Term term in Terms)
            {
                if (System.Math.Abs(term.Power - 1.0) > 0.000001 && System.Math.Abs(term.Power) > 0.000001)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Finds root specified interval using Newton's method
        /// </summary>
        /// <param name="min">Start point.</param>
        /// <param name="max">End point.</param>
        /// <param name="derivative">The derivative of the polinomial.</param>
        /// <returns></returns>
        private double findroot(double min, double max, Poly derivative)
        {
            //initialize previous value
            double previous = (min + max) / 2;
            double tolerance = 0.00001;
            double root = Double.MaxValue;
            double calculate = Double.MaxValue;

            while (System.Math.Abs(previous - root) > tolerance)
            {
                if (root < Double.MaxValue)
                {
                    previous = root;
                }             
                root = previous - Calculate(previous) / derivative.Calculate(previous);
            }
            return root;
        }

        #endregion

        #region Fields & Properties:

        /// <summary>
        /// Terms Property, ObjectType of TermCollection
        /// </summary>
        private TermCollection _Terms;

        public TermCollection Terms
        {
            get
            {
                return _Terms;
            }
            set
            {
                _Terms = value;
            }
        }

        private double _startpoint;

        private double _endpoint;

        /// <summary>
        /// Read-Only Property return the Length of TermCollection which means length of Polynomial Expression.
        /// </summary>
        public int Lentgh
        {
            get
            {
                return this.Terms.Length;
            }
        }

        public double StartPoint
        {
            get { return _startpoint; }
            set { _startpoint = value; }
        }

        public double EndPoint
        {
            get { return _endpoint; }
            set { _endpoint = value; }
        }

        #endregion

        #region Operator OverLoading:

        /// <summary>
        /// Plus Operator: 
        /// Add Method of TermsCollection will Check the Power of each Term And if it's already 
        /// exists in the Collection Just Plus the Coefficient of the Term and This Mean Plus Operation.
        /// So We Simply Add the Terms of Second Poly to the First one.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Poly operator +(Poly p1, Poly p2)
        {
            if (p1.ToString() == "0")
            {
                return p2;
            }
            else if (p2.ToString() == "0")
            {
                return p1;
            }

            Poly result = new Poly(p1.ToString());
            foreach (Term t in p2.Terms)
                result.Terms.Add(t);
            return result;
        }

        /// <summary>
        /// Minus Operations: Like Plus Operation but at first we just Make the Second Poly to the Negetive Value.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Poly operator -(Poly p1, Poly p2)
        {
            if (p1.ToString() == "0")
            {
                return -1*p2;
            }
            else if (p2.ToString() == "0")
            {
                return p1;
            }

            Poly result = new Poly(p1.ToString());
            Poly NegetiveP2 = new Poly(p2.ToString());
            foreach (Term t in NegetiveP2.Terms)
                t.Coefficient *= -1;

            return result + NegetiveP2;
        }

        /// <summary>
        /// Multiple Operation: For each term in the First Poly We Multiple it in the Each Term of Second Poly
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Poly operator *(Poly p1, Poly p2)
        {
            TermCollection result = new TermCollection();
            foreach (Term t1 in p1.Terms)
            {
                foreach (Term t2 in p2.Terms)
                {
                    result.Add(new Term(t1.Power + t2.Power, t1.Coefficient * t2.Coefficient));
                }
            }
            return new Poly(result);
        }

        /// <summary>
        /// Multiple with a scalar
        /// </summary>
        /// <param name="p1">The polynomial to be multiplied</param>
        /// <param name="v1">The double number to multiply</param>
        /// <returns>Multiplied polynomial</returns>
        public static Poly operator *(Poly p1, double v1)
        {
            var result = new TermCollection();
            foreach (Term t in p1.Terms)
            {
                Term newterm = new Term(t.Power, t.Coefficient * v1);
                result.Add(newterm);
            }
            return new Poly(result, p1.StartPoint, p1.EndPoint);
        }

        /// <summary>
        /// Divide operation.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Poly operator /(Poly p1, Poly p2)
        {
            p1.Terms.Sort(TermCollection.SortType.DES);
            p2.Terms.Sort(TermCollection.SortType.DES);
            TermCollection resultTerms = new TermCollection();
            if (p1.Terms[0].Power < p2.Terms[0].Power)
                throw new Exception("Invalid Division: P1.MaxPower is Lower than P2.MaxPower");
            while (p1.Terms[0].Power > p2.Terms[0].Power)
            {
                Term NextResult = new Term(p1.Terms[0].Power - p2.Terms[0].Power, p1.Terms[0].Coefficient / p2.Terms[0].Coefficient);
                resultTerms.Add(NextResult);
                Poly TempPoly = NextResult;

                Poly NewPoly = TempPoly * p2;
                p1 = p1 - NewPoly;
            }
            return new Poly(resultTerms);
        }

        /// <summary>
        /// Divide operation with a scalar.
        /// </summary>
        /// <param name="p1">The polynomial to be devided</param>
        /// <param name="v1">The double number to devide</param>
        /// <returns>Devided polynomial</returns>
        public static Poly operator /(Poly p1, double v1)
        {
            var result = new TermCollection();
            foreach (Term t in p1.Terms)
            {
                Term newterm = new Term(t.Power, t.Coefficient/v1);
                result.Add(newterm);
            }
            return new Poly(result, p1.StartPoint, p1.EndPoint);
        }

        /// <summary>
        /// this will Create a new Poly by the Value of 1 and Plus it to the First Poly.
        /// </summary>
        /// <param name="p1"></param>
        /// <returns></returns>
        public static Poly operator ++(Poly p1)
        {
            Poly p2 = new Poly("1");
            p1 = p1 + p2;
            return p1;
        }

        /// <summary>
        /// this will Create a new Poly by the Value of -1 and Plus it to the First Poly.
        /// </summary>
        /// <param name="p1"></param>
        /// <returns></returns>
        public static Poly operator --(Poly p1)
        {
            Poly p2 = new Poly("-1");
            p1 = p1 + p2;
            return p1;
        }

        /// <summary>
        /// Implicit Conversion : this will Convert the single Term to the Poly. 
        /// First it Creates a new Instance of TermCollection and Add The Term to it. 
        /// Second Creates a new Poly by the TermCollection and Return it.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static implicit operator Poly(Term t)
        {
            TermCollection Terms = new TermCollection();
            Terms.Add(t);
            return new Poly(Terms);
        }

        /// <summary>
        /// Implicit Conversion: this will Create new Instance of Poly by the String Constructor
        /// And return it.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static implicit operator Poly(string expression)
        {
            return new Poly(expression);
        }

        /// <summary>
        /// Implicit Conversion: this will Create new Instance of Poly by the String Constructor
        /// And return it.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Poly(int value)
        {
            return new Poly(value.ToString());
        }
        #endregion
    }
}
