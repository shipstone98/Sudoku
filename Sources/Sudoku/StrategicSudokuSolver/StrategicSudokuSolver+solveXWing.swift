//
//  StrategicSudokuSolver+solveXWing.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 31/03/2026.
//

import Utilities

internal extension StrategicSudokuSolver {
    func solveXWing<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        self.solveRow(using: &generator) ?? self.solveColumn(using: &generator)
    }
    
    private func solveColumn<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        for column1 in (0..<8).shuffled(using: &generator) {
            for column2 in ((column1 + 1)..<9).shuffled(using: &generator) {
                for candidate in getCandidates(using: &generator) {
                    var column1Rows: Set<Int> = []
                    var column2Rows: Set<Int> = []
                    
                    for row in 0..<9 {
                        if let candidates = self.candidates[row * 9 + column1] {
                            if candidates.contains(candidate) {
                                column1Rows.insert(row)
                            }
                            
                            if candidates.contains(candidate) {
                                column1Rows.insert(row)
                            }
                        }
                        
                        if let candidates = self.candidates[row * 9 + column2] {
                            if candidates.contains(candidate) {
                                column2Rows.insert(row)
                            }
                            
                            if candidates.contains(candidate) {
                                column2Rows.insert(row)
                            }
                        }
                    }
                    
                    guard column1Rows.count == 2,
                          column1Rows == column2Rows else {
                        continue
                    }
                    
                    let row1 = column1Rows.first!
                    let row2 = column1Rows.last!
                    var locations: Set<SudokuSolverMove.Location> = []
                    let removedCandidates: Set<Int> = [candidate]
                    
                    for column in 0..<9 {
                        guard !(column == column1 || column == column2) else {
                            continue
                        }
                        
                        if let candidates = self.candidates[row1 * 9 + column],
                           candidates.contains(candidate) {
                            let location = SudokuSolverMove.Location(
                                row1,
                                column,
                                removedCandidates
                            )
                            
                            locations.insert(location)
                        }
                        
                        if let candidates = self.candidates[row2 * 9 + column],
                           candidates.contains(candidate) {
                            let location = SudokuSolverMove.Location(
                                row2,
                                column,
                                removedCandidates
                            )
                            
                            locations.insert(location)
                        }
                    }
                    
                    guard !locations.isEmpty else {
                        continue
                    }
                    
                    return .init(for: .xWing, at: locations)
                }
            }
        }
        
        return nil
    }
    
    private func solveRow<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        for row1 in (0..<8).shuffled(using: &generator) {
            for row2 in ((row1 + 1)..<9).shuffled(using: &generator) {
                for candidate in getCandidates(using: &generator) {
                    var row1Columns: Set<Int> = []
                    var row2Columns: Set<Int> = []
                    
                    for column in 0..<9 {
                        if let candidates = self.candidates[row1 * 9 + column] {
                            if candidates.contains(candidate) {
                                row1Columns.insert(column)
                            }
                            
                            if candidates.contains(candidate) {
                                row1Columns.insert(column)
                            }
                        }
                        
                        if let candidates = self.candidates[row2 * 9 + column] {
                            if candidates.contains(candidate) {
                                row2Columns.insert(column)
                            }
                            
                            if candidates.contains(candidate) {
                                row2Columns.insert(column)
                            }
                        }
                    }
                    
                    guard row1Columns.count == 2,
                          row1Columns == row2Columns else {
                        continue
                    }
                    
                    let column1 = row1Columns.first!
                    let column2 = row1Columns.last!
                    var locations: Set<SudokuSolverMove.Location> = []
                    let removedCandidates: Set<Int> = [candidate]
                    
                    for row in 0..<9 {
                        guard !(row == row1 || row == row2) else {
                            continue
                        }
                        
                        if let candidates = self.candidates[row * 9 + column1],
                           candidates.contains(candidate) {
                            let location = SudokuSolverMove.Location(
                                row,
                                column1,
                                removedCandidates
                            )
                            
                            locations.insert(location)
                        }
                        
                        if let candidates = self.candidates[row * 9 + column2],
                           candidates.contains(candidate) {
                            let location = SudokuSolverMove.Location(
                                row,
                                column2,
                                removedCandidates
                            )
                            
                            locations.insert(location)
                        }
                    }
                    
                    guard !locations.isEmpty else {
                        continue
                    }
                    
                    return .init(for: .xWing, at: locations)
                }
            }
        }
        
        return nil
    }
}
