﻿using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;

namespace Cauldron.Interception.Cecilator
{
    internal class InstructionContainer : IList<Instruction>
    {
        private readonly List<ExceptionHandler> exceptionHandlers = new List<ExceptionHandler>();
        private readonly List<Instruction> instruction = new List<Instruction>();

        public InstructionContainer()
        {
        }

        private InstructionContainer(InstructionContainer a, InstructionContainer b, Mono.Collections.Generic.Collection<VariableDefinition> variables)
        {
            this.instruction.AddRange(a.instruction);
            this.instruction.AddRange(b.instruction);
        }

        private InstructionContainer(InstructionContainer a, IEnumerable<Instruction> b, Mono.Collections.Generic.Collection<VariableDefinition> variables)
        {
            this.instruction.AddRange(a.instruction);
            this.instruction.AddRange(b);
        }

        public int Count { get { return this.instruction.Count; } }

        public List<ExceptionHandler> ExceptionHandlers { get { return this.exceptionHandlers; } }

        bool ICollection<Instruction>.IsReadOnly => false;

        public Instruction this[int index] { get { return this.instruction[index]; } }

        Instruction IList<Instruction>.this[int index]
        {
            get => this[index];
            set { }
        }

        public static implicit operator Instruction[] (InstructionContainer a) => a.instruction.ToArray();

        public void Append(Instruction instruction) => this.instruction.Add(instruction);

        public void Append(IEnumerable<Instruction> instructions) => this.instruction.AddRange(instructions);

        public void Clear()
        {
            this.instruction.Clear();
            this.exceptionHandlers.Clear();
        }

        public InstructionContainer Clone() => new InstructionContainer();

        public Instruction First() => this.instruction.Count == 0 ? null : this.instruction[0];

        public IEnumerator<Instruction> GetEnumerator() => this.instruction.GetEnumerator();

        public int IndexOf(Instruction item) => this.instruction.IndexOf(item);

        public void Insert(int index, Instruction item) => this.instruction.Insert(index, item);

        public void Insert(int index, IEnumerable<Instruction> items)
        {
            foreach (var item in items)
                this.instruction.Insert(index++, item);
        }

        public void InsertAfter(Instruction position, Instruction instructionToInsert)
        {
            var index = this.instruction.IndexOf(position) + 1;

            if (index == this.instruction.Count)
                this.instruction.Add(instructionToInsert);
            else
                this.instruction.Insert(index, instructionToInsert);
        }

        public Instruction Last() => this.instruction.Count == 0 ? null : this.instruction[this.instruction.Count - 1];

        public Instruction Next(Instruction instruction)
        {
            var index = this.instruction.IndexOf(instruction);
            return this.instruction[index + 1];
        }

        public void RemoveLast() => this.instruction.RemoveAt(this.instruction.Count - 1);

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var item in this.instruction)
                sb.AppendLine($"IL_{item.Offset.ToString("X4")}: {item.OpCode.ToString()} { (item.Operand is Instruction ? "IL_" + (item.Operand as Instruction).Offset.ToString("X4") : item.Operand?.ToString())} ");

            return sb.ToString();
        }

        void ICollection<Instruction>.Add(Instruction item) => this.instruction.Add(item);

        bool ICollection<Instruction>.Contains(Instruction item) => this.instruction.Contains(item);

        void ICollection<Instruction>.CopyTo(Instruction[] array, int arrayIndex) => this.instruction.CopyTo(array, arrayIndex);

        IEnumerator IEnumerable.GetEnumerator() => this.instruction.GetEnumerator();

        bool ICollection<Instruction>.Remove(Instruction item) => this.instruction.Remove(item);

        void IList<Instruction>.RemoveAt(int index) => this.instruction.RemoveAt(index);

        internal Instruction[] ToArray() => this.instruction.ToArray();
    }
}