using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
    public class EffectPass
    {
        EffectTechnique _technique = null;
		internal int shaderProgram;
		int glExt = 0;
		
		
		public void Apply ()
		{

			// Tell the GL Context to use the program
			GL.UseProgram (shaderProgram);	

			_technique._effect.OnApply();

		}

        public EffectPass(EffectTechnique technique)
        {
            _technique = technique;
			// Choose the appropriate shader4 GL call based on the available extensions
			var extensions = new HashSet<string>(GL.GetString(StringName.Extensions).Split(new char[] { ' ' }));
			
			if (extensions.Contains("GL_EXT_geometry_shader4"))
			{
				glExt = 1;
			}
			else if (extensions.Contains("GL_ARB_geometry_shader4"))
			{
				glExt = 2;
			}
			else
			{
				glExt = 0;
			}
        }
		
		internal void ApplyPass ()
		{
			
			// Create a Program object
			shaderProgram = GL.CreateProgram ();

			// Attach our compiled shaders
			if ( VertexIndex < _technique._effect.vertexShaders.Count)
				GL.AttachShader (shaderProgram, _technique._effect.vertexShaders[VertexIndex]);
			if ( FragmentIndex < _technique._effect.fragmentShaders.Count)			
				GL.AttachShader (shaderProgram, _technique._effect.fragmentShaders[FragmentIndex]);
			
			// Set the parameters
			if (glExt == 1)
			{
				GL.Ext.ProgramParameter (shaderProgram, ExtGeometryShader4.GeometryInputTypeExt, (int)All.Lines);
				GL.Ext.ProgramParameter (shaderProgram, ExtGeometryShader4.GeometryOutputTypeExt, (int)All.Line);
			}
			else if (glExt == 2)
			{
				GL.Arb.ProgramParameter (shaderProgram, ArbGeometryShader4.GeometryInputTypeArb, (int)All.Lines);	
				GL.Arb.ProgramParameter (shaderProgram, ArbGeometryShader4.GeometryOutputTypeArb, (int)All.Line);				
			}
			else
			{
				GL.ProgramParameter (shaderProgram, AssemblyProgramParameterArb.GeometryInputType, (int)All.Lines);
				GL.ProgramParameter (shaderProgram, AssemblyProgramParameterArb.GeometryOutputType, (int)All.Line);
			}

			// Set the max vertices
			int maxVertices;
			GL.GetInteger (GetPName.MaxGeometryOutputVertices, out maxVertices);
			
			if (glExt == 1)
				GL.Ext.ProgramParameter (shaderProgram, ExtGeometryShader4.MaxGeometryOutputVerticesExt, maxVertices);
			else if (glExt == 2)
				GL.Arb.ProgramParameter (shaderProgram, ArbGeometryShader4.GeometryVerticesOutArb, maxVertices);
			else
				GL.ProgramParameter (shaderProgram, AssemblyProgramParameterArb.GeometryVerticesOut, maxVertices);

			// Link the program
			GL.LinkProgram (shaderProgram);
			string name = String.Format("Technique {0} - Pass {1}: ",_technique.Name, Name);
			ShaderLog(name,shaderProgram);				
			
		}

		internal void UpdatePass(int shader, int fragment)
		{
			int count = 0;
			//int[] objs = new int[10];
			int obj = 0;
			int max = 10;
			// Detach all the shaders
			GL.GetAttachedShaders(shaderProgram,max, out count, out obj);
			while (count > 0) {
				GL.DetachShader(shaderProgram, obj);
				GL.GetAttachedShaders(shaderProgram,max, out count, out obj);
			}
//			for (int x = 0; x < count; x++) {
//				GL.DetachShader(shaderProgram, obj);
//			}

			// Attach our compiled shaders
			if ( shader > 0)
				GL.AttachShader (shaderProgram, shader);
			if ( fragment > 0)
				GL.AttachShader (shaderProgram, fragment);

			// Set the parameters
			if (glExt == 1)
			{
				GL.Ext.ProgramParameter (shaderProgram, ExtGeometryShader4.GeometryInputTypeExt, (int)All.Lines);
				GL.Ext.ProgramParameter (shaderProgram, ExtGeometryShader4.GeometryOutputTypeExt, (int)All.Line);
			}
			else if (glExt == 2)
			{
				GL.Arb.ProgramParameter (shaderProgram, ArbGeometryShader4.GeometryInputTypeArb, (int)All.Lines);	
				GL.Arb.ProgramParameter (shaderProgram, ArbGeometryShader4.GeometryOutputTypeArb, (int)All.Line);				
			}
			else
			{
				GL.ProgramParameter (shaderProgram, AssemblyProgramParameterArb.GeometryInputType, (int)All.Lines);
				GL.ProgramParameter (shaderProgram, AssemblyProgramParameterArb.GeometryOutputType, (int)All.Line);
			}

			// Set the max vertices
			int maxVertices;
			GL.GetInteger (GetPName.MaxGeometryOutputVertices, out maxVertices);

			if (glExt == 1)
				GL.Ext.ProgramParameter (shaderProgram, ExtGeometryShader4.MaxGeometryOutputVerticesExt, maxVertices);
			else if (glExt == 2)
				GL.Arb.ProgramParameter (shaderProgram, ArbGeometryShader4.GeometryVerticesOutArb, maxVertices);
			else
				GL.ProgramParameter (shaderProgram, AssemblyProgramParameterArb.GeometryVerticesOut, maxVertices);

			// Link the program
			GL.LinkProgram (shaderProgram);
			string name = String.Format("Technique {0} - Pass {1}: ",_technique.Name, Name);
			ShaderLog(name,shaderProgram);
			GL.UseProgram(shaderProgram);
		}
		
		public string Name { get; set; }
		
		// internal for now until I figure out what I can do with this mess
		internal int VertexIndex { get; set; }
		internal int FragmentIndex { get; set; }
		
		// Output the log of an object
		private void ShaderLog (string whichObj, int obj)
		{
			int infoLogLen = 0;
			var infoLog = "Is good to go.";

			GL.GetProgram (obj, ProgramParameter.InfoLogLength, out infoLogLen);

			if (infoLogLen > 0)
				infoLog = GL.GetProgramInfoLog (obj);

			Console.WriteLine ("{0} {1}", whichObj, infoLog);

		}		
    }
}
