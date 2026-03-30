//
//  StrategicSudokuSolver.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

@frozen
public struct StrategicSudokuSolver : Codable, Hashable, Sendable, SudokuSolver {
    internal private(set) var candidates: [Int: Set<Int>]
    public private(set) var moves: [SudokuSolverMove]
    public private(set) var sudoku: ArraySudoku
    
    public init<S>(_ sudoku: S) where S : SudokuProtocol {
        let sudoku = ArraySudoku(sudoku)
        var candidates: [Int: Set<Int>] = [:]
        
        for row in 0..<9 {
            for column in 0..<9 {
                let index = row * 9 + column
                
                guard sudoku.array[index] == 0 else {
                    continue
                }
                
                candidates[index] = sudoku.candidates(row, column)
            }
        }
        
        self.candidates = candidates
        self.moves = []
        self.sudoku = sudoku
    }
    
    @discardableResult
    public mutating func solve<T>(using generator: inout T) -> Bool where T : RandomNumberGenerator {
        var isSolved: Bool
        
        repeat {
            isSolved = SudokuSolverStrategy.allCases.contains {
                self.solve(for: $0, using: &generator) != nil
            }
        } while isSolved
        
        return self.sudoku.array.allSatisfy { $0 > 0 }
    }
    
    @discardableResult
    public mutating func solve<T>(
        for strategy: SudokuSolverStrategy,
        using generator: inout T
    ) -> SudokuSolverMove? where T : RandomNumberGenerator {
        let move: SudokuSolverMove?
        
        switch strategy {
        case .fullHouse:
            move = self.solveFullHouse(using: &generator)
        case .nakedSingle:
            move = self.solveNakedSingle(using: &generator)
        case .hiddenSingle:
            move = self.solveHiddenSingle(using: &generator)
        case .pointingCandidate:
            move = self.solvePointingCandidate(using: &generator)
        case .claimingCandidate:
            move = self.solveClaimingCandidate(using: &generator)
        case .nakedPair:
            move = self.solveNakedPair(using: &generator)
        }
        
        guard let move else {
            return nil
        }
        
        for location in move.locations {
            let index = location.row * 9 + location.column
            
            if let addedValue = location.addedValue {
                for index in ArraySudoku.peers(location.row, location.column) {
                    if var candidates = self.candidates[index] {
                        candidates.remove(addedValue)
                        self.candidates[index] = candidates
                    }
                }
                
                self.candidates.removeValue(forKey: index)
                self.sudoku.array[index] = addedValue
            } else {
                if var candidates = self.candidates[index] {
                    for candidate in location.removedCandidates {
                        candidates.remove(candidate)
                    }
                    
                    self.candidates[index] = candidates
                }
            }
        }
        
        self.moves.append(move)
        return move
    }
}
