//
//  SudokuSolverMove.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

@frozen
public struct SudokuSolverMove : Codable, Hashable, Sendable {
    public let locations: Set<Location>
    public let strategy: SudokuSolverStrategy?
    
    internal init(for strategy: SudokuSolverStrategy?, at location: Location) {
        self.locations = [location]
        self.strategy = strategy
    }
    
    internal init<S>(for strategy: SudokuSolverStrategy?, at locations: S) where S : Sequence, S.Element == Location {
        self.locations = .init(locations)
        self.strategy = strategy
    }
    
    @frozen
    public struct Location : Codable, Hashable, Sendable {
        public let addedValue: Int?
        public let column: Int
        public let removedCandidates: Set<Int>
        public let row: Int
        
        internal init(_ row: Int, _ column: Int, _ removedCandidates: Set<Int>) {
            self.addedValue = nil
            self.column = column
            self.removedCandidates = removedCandidates
            self.row = row
        }
        
        internal init(_ row: Int, _ column: Int, addedValue: Int) {
            self.addedValue = addedValue
            self.column = column
            self.removedCandidates = []
            self.row = row
        }
    }
}
