using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace TkLearning.Primitives
{
    public abstract class Primitive
    {
        public Vector3[] Verts = Array.Empty<Vector3>();
        public Vector2[] TexCoords = Array.Empty<Vector2>();
        public uint[] Indices = Array.Empty<uint>();
        
        int VertexBufferObject;
        int UVBufferObject;

        int VertexArrayObject;
        int ElementBufferObject;
        Shader ShaderProgram;

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

void main()
{
    texCoord = aTexCoord;
    gl_Position = vec4(aPosition, 1.0);
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

            Console.WriteLine("Primitive Installed.");
        }

        public void Draw()
        {
            ShaderProgram.Use();
            GL.BindVertexArray(VertexArrayObject);
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
        }
    }

    public class Pyramid : Primitive
    {

    }

    public class Cube : Primitive
    {

    }
}
