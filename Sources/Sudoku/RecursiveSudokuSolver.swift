//
//  RecursiveSudokuSolver.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

@frozen
public struct RecursiveSudokuSolver : Codable, Hashable, Sendable, SudokuSolver {
    public private(set) var moves: [SudokuSolverMove]
    public private(set) var sudoku: ArraySudoku
    
    internal init() {
        self.moves = []
        self.sudoku = .init()
    }
    
    public init<S>(_ sudoku: S) where S : SudokuProtocol {
        self.moves = []
        self.sudoku = .init(sudoku)
    }
    
    @discardableResult
    public mutating func solve<T>(using generator: inout T) -> Bool where T : RandomNumberGenerator {
        let indices = getIndices(using: &generator)
        return self.solve(using: &generator, indices, at: 0)
    }
    
    private mutating func solve<T>(
        using generator: inout T,
        _ indices: [Int],
        at indicesIndex: Int
    ) -> Bool where T : RandomNumberGenerator {
        let index = indices[indicesIndex]
        
        guard self.sudoku.array[index] == 0 else {
            return self.solveNext(using: &generator, indices, at: indicesIndex)
        }
        
        let row = index / 9
        let column = index % 9
        
        let candidates = self.sudoku.candidates(row, column)
            .sorted()
            .shuffled(using: &generator)
        
        for candidate in candidates {
            self.sudoku.array[index] = candidate
            
            let location = SudokuSolverMove.Location(
                row,
                column,
                addedValue: candidate
            )
            
            let move = SudokuSolverMove(for: nil, at: location)
            self.moves.append(move)
            
            if self.solveNext(using: &generator, indices, at: indicesIndex) {
                return true
            }
            
            self.moves.removeLast()
        }
        
        self.sudoku.array[index] = 0
        return false
    }
    
    private mutating func solveNext<T>(
        using generator: inout T,
        _ indices: [Int],
        at indicesIndex: Int
    ) -> Bool where T : RandomNumberGenerator {
        let indicesIndex = indicesIndex + 1
        
        guard indicesIndex < indices.count else {
            return true
        }
        
        return self.solve(using: &generator, indices, at: indicesIndex)
    }
}
