//
//  SudokuSolverStrategy.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

public enum SudokuSolverStrategy : BitwiseCopyable, CaseIterable, Codable, Comparable, Hashable, Sendable {
    case fullHouse
    case nakedSingle
    case hiddenSingle
    case pointingCandidate
    case claimingCandidate
    case nakedPair
}
