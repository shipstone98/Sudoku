//
//  StrategicSudokuSolver+solveBUGPlus1.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 31/03/2026.
//

import Utilities

internal extension StrategicSudokuSolver {
    func solveBUGPlus1() -> SudokuSolverMove? {
        var indices: Set<Int> = []
        
        for (index, candidates) in self.candidates {
            if candidates.count == 3 {
                indices.insert(index)
            } else {
                guard candidates.count == 2 else {
                    return nil
                }
            }
        }
        
        guard let index = indices.single else {
            return nil
        }
        
        let row = index / 9
        let column = index % 9
        
        for candidate in self.candidates[index]! {
            var rowColumns: Set<Int> = []
            var columnRows: Set<Int> = []
            
            for index in 0..<9 {
                if let candidates = self.candidates[row * 9 + index],
                   candidates.contains(candidate) {
                    rowColumns.insert(index)
                }
                
                if let candidates = self.candidates[index * 9 + column],
                   candidates.contains(candidate) {
                    columnRows.insert(index)
                }
            }
            
            if rowColumns.count < 3 && columnRows.count < 3 {
                let blockRow = row - row % 3
                let blockColumn = column - column % 3
                var blockIndices: Set<Int> = []
                
                for rowOffset in 0..<3 {
                    let currentRow = blockRow + rowOffset
                    
                    for columnOffset in 0..<3 {
                        let index = currentRow * 9 + blockColumn + columnOffset
                        
                        if let candidates = self.candidates[index],
                           candidates.contains(candidate) {
                            blockIndices.insert(index)
                        }
                    }
                }
                
                guard blockIndices.count > 2 else {
                    continue
                }
            }
            
            let location = SudokuSolverMove.Location(
                row,
                column,
                addedValue: candidate
            )
            
            return .init(for: .bugPlus1, at: location)
        }
        
        return nil
    }
}
