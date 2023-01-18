using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using static System.Formats.Asn1.AsnWriter;
using System.Diagnostics;

namespace TkLearning.Primitives
{
    public abstract class Primitive
    {
        public Vector3[] Verts = Array.Empty<Vector3>();
        public Vector2[] TexCoords = Array.Empty<Vector2>();
        public uint[] Indices = Array.Empty<uint>();
        public string ColorMapPath = "";

        public int VertexBufferObject;
        public int UVBufferObject;

        public int VertexArrayObject;
        public int ElementBufferObject;
        public Shader ShaderProgram;
        public Texture ColorMap;

        public Matrix4 rotation;
        public Matrix4 scale;
        public Matrix4 position;
        public Matrix4 trans;


        public Primitive() { }

        public void Load()
        {
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Verts.Length * Vector3.SizeInBytes, Verts, BufferUsageHint.StaticDraw);

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
            GL.EnableVertexAttribArray(0);

            UVBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, UVBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, TexCoords.Length * Vector2.SizeInBytes, TexCoords, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);
            GL.EnableVertexAttribArray(1);

            // ElementBuffer binding requires an actively bound VAO 
            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);

            string vertShader = @"
#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;

out vec2 texCoord;

uniform mat4 transform;

void main()
{
    texCoord = aTexCoord;
    gl_Position = vec4(aPosition, 1.0) * transform;
}
";
            string fragShader = @"
#version 330 core

out vec4 FragColor;

in vec2 texCoord;

uniform sampler2D texture0;

void main()
{
    // FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
    FragColor = texture(texture0, texCoord);
}
";

            ShaderProgram = new Shader(vertShader, fragShader);

            ShaderProgram.Use();

            if(ColorMapPath.Length > 0)
            {
                ColorMap = new Texture(ColorMapPath);
            }

            rotation = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(0));
            scale = Matrix4.CreateScale(1.0f, 1.0f, 1.0f);
            position = Matrix4.CreateTranslation(0, 0, 0);
            trans = rotation * scale * position;

            Console.WriteLine("Primitive Installed.");
        }

        public virtual void Update(float Time)
        {
            
            
        }

        public void Draw(float Time)
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            ShaderProgram.Use();

            trans = ((rotation * scale) * position) * Time;

            int transLocation = GL.GetUniformLocation(ShaderProgram.Handle, "transform");
            GL.UniformMatrix4(transLocation, true, ref trans);

            GL.BindVertexArray(VertexArrayObject);
            
            ColorMap.Use();

            GL.DrawElements(PrimitiveType.TriangleFan, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }

    public class Triangle : Primitive
    {
        public Triangle() : base()
        {
            Verts = new Vector3[]
            {
                (-0.5f, -0.5f, 0.0f), // BL
                (0.5f, -0.5f, 0.0f), // BR
                (0.0f, 0.5f, 0.0f), // T
            };

            TexCoords = new Vector2[]
            {
                (0.0f, 0.0f), // Lower Left
                (1.0f, 0.0f), // Lower Right
                (0.5f, 1.0f), // Top Center
            };

            Indices = new uint[]
            {
                0, 1, 2
            };

            //ColorMap = new Texture("container.jpg");
        }
    }

    public class Quad : Primitive
    {
        public Quad() : base()
        {
            Verts = new Vector3[]
            {
                (0.5f, 0.5f, 0.0f),   // TR
                (0.5f, -0.5f, 0.0f),  // BR
                (-0.5f, -0.5f, 0.0f), // BL
                (-0.5f, 0.5f, 0.0f)   // TL
            };

            TexCoords = new Vector2[]
            {
                (1.0f, 1.0f), // TR
                (1.0f, 0.0f), // BR
                (0.0f, 0.0f), // BL
                (0.0f, 1.0f), // TL
            };

            Indices = new uint[]
            {
                0, 1, 3,
                1, 2, 3
            };

            ColorMapPath = "container.jpg";
        }

        public override void Update(float Time)
        {
            base.Update(Time);

            //Console.WriteLine(Time);

            //rotation = rotation * (Matrix4.CreateRotationY(MathHelper.DegreesToRadians(0.5f)) * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(0.5f)));
            scale = Matrix4.CreateScale(0.5f, 0.5f, 0.5f);
            position *= Matrix4.CreateTranslation(0.0005f, 0.0f, 0.0f);
            //trans = ((rotation * scale) * Matrix4.CreateTranslation(0.5f, 0.0f, 0.0f) ) * Time;
        }
    }

    public class Pyramid : Primitive
    {

    }

    public class Cube : Primitive
    {
        public Cube() : base()
        {
            Verts = new Vector3[]
            {
                (-0.5f, -0.5f, -0.5f),
                ( 0.5f, -0.5f, -0.5f),
                ( 0.5f,  0.5f, -0.5f),
                ( 0.5f,  0.5f, -0.5f),
                (-0.5f,  0.5f, -0.5f),
                (-0.5f, -0.5f, -0.5f),

                (-0.5f, -0.5f,  0.5f),
                ( 0.5f, -0.5f,  0.5f),
                ( 0.5f,  0.5f,  0.5f),
                ( 0.5f,  0.5f,  0.5f),
                (-0.5f,  0.5f,  0.5f),
                (-0.5f, -0.5f,  0.5f),

                (-0.5f,  0.5f,  0.5f),
                (-0.5f,  0.5f, -0.5f),
                (-0.5f, -0.5f, -0.5f),
                (-0.5f, -0.5f, -0.5f),
                (-0.5f, -0.5f,  0.5f),
                (-0.5f,  0.5f,  0.5f),

                ( 0.5f,  0.5f,  0.5f),
                ( 0.5f,  0.5f, -0.5f),
                ( 0.5f, -0.5f, -0.5f),
                ( 0.5f, -0.5f, -0.5f),
                ( 0.5f, -0.5f,  0.5f),
                ( 0.5f,  0.5f,  0.5f),

                (-0.5f, -0.5f, -0.5f),
                ( 0.5f, -0.5f, -0.5f),
                ( 0.5f, -0.5f,  0.5f),
                ( 0.5f, -0.5f,  0.5f),
                (-0.5f, -0.5f,  0.5f),
                (-0.5f, -0.5f, -0.5f),

                (-0.5f,  0.5f, -0.5f),
                ( 0.5f,  0.5f, -0.5f),
                ( 0.5f,  0.5f,  0.5f),
                ( 0.5f,  0.5f,  0.5f),
                (-0.5f,  0.5f,  0.5f),
                (-0.5f,  0.5f, -0.5f),
            };

            TexCoords = new Vector2[]
            {
                (0.0f, 0.0f),
                (1.0f, 0.0f),
                (1.0f, 1.0f),
                (1.0f, 1.0f),
                (0.0f, 1.0f),
                (0.0f, 0.0f),
                (0.0f, 0.0f),
                (1.0f, 0.0f),
                (1.0f, 1.0f),
                (1.0f, 1.0f),
                (0.0f, 1.0f),
                (0.0f, 0.0f),
                (1.0f, 0.0f),
                (1.0f, 1.0f),
                (0.0f, 1.0f),
                (0.0f, 1.0f),
                (0.0f, 0.0f),
                (1.0f, 0.0f),
                (1.0f, 0.0f),
                (1.0f, 1.0f),
                (0.0f, 1.0f),
                (0.0f, 1.0f),
                (0.0f, 0.0f),
                (1.0f, 0.0f),
                (0.0f, 1.0f),
                (1.0f, 1.0f),
                (1.0f, 0.0f),
                (1.0f, 0.0f),
                (0.0f, 0.0f),
                (0.0f, 1.0f),
                (0.0f, 1.0f),
                (1.0f, 1.0f),
                (1.0f, 0.0f),
                (1.0f, 0.0f),
                (0.0f, 0.0f),
                (0.0f, 1.0f)
            };

            Indices = new uint[]
            {
            };

            ColorMapPath = "container.jpg";
        }

        public void Draw2()
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            ShaderProgram.Use();

            int transLocation = GL.GetUniformLocation(ShaderProgram.Handle, "transform");
            GL.UniformMatrix4(transLocation, true, ref trans);

            GL.BindVertexArray(VertexArrayObject);

            ColorMap.Use();

            GL.DrawArrays(PrimitiveType.Triangles, 0, Verts.Length);
        }
    }
}
