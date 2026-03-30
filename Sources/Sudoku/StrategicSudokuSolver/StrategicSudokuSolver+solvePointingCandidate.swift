//
//  StrategicSudokuSolver+solvePointingCandidate.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

internal extension StrategicSudokuSolver {
    func solvePointingCandidate<T>(using generator: inout T) -> SudokuSolverMove? where T : RandomNumberGenerator {
        for blockRow in getBlockIndices(using: &generator) {
            for blockColumn in getBlockIndices(using: &generator) {
                for candidate in getCandidates(using: &generator) {
                    var rows: Set<Int> = []
                    var columns: Set<Int> = []
                    
                    for rowOffset in 0..<3 {
                        let row = blockRow + rowOffset
                        
                        for columnOffset in 0..<3 {
                            let column = blockColumn + columnOffset
                            
                            if let candidates = self.candidates[row * 9 + column],
                               candidates.contains(candidate) {
                                rows.insert(row)
                                columns.insert(column)
                            }
                        }
                    }
                    
                    var indices: Set<Int> = []
                    
                    if let row = rows.single {
                        for column in 0..<9 {
                            guard column - column % 3 != blockColumn else {
                                continue
                            }
                            
                            let index = row * 9 + column
                            
                            if let candidates = self.candidates[index],
                               candidates.contains(candidate) {
                                indices.insert(index)
                            }
                        }
                    } else if let column = columns.single {
                        for row in 0..<9 {
                            guard row - row % 3 != blockRow else {
                                continue
                            }
                            
                            let index = row * 9 + column
                            
                            if let candidates = self.candidates[index],
                               candidates.contains(candidate) {
                                indices.insert(index)
                            }
                        }
                    }
                    
                    guard !indices.isEmpty else {
                        continue
                    }
                    
                    let removedCandidates: Set<Int> = [candidate]
                    
                    let locations = indices.map {
                        SudokuSolverMove.Location(
                            $0 / 9,
                            $0 % 9,
                            removedCandidates
                        )
                    }
                    
                    return .init(for: .pointingCandidate, at: locations)
                }
            }
        }
        
        return nil
    }
}
